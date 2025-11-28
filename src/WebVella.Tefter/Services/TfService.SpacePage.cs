using Microsoft.AspNetCore.Routing.Template;
using Nito.AsyncEx.Synchronous;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	public List<TfSpacePage> GetAllSpacePages();

	public List<TfSpacePage> GetSpacePages(
		Guid spaceId);

	public TfSpacePage GetSpacePage(
		Guid pageId);

	public (Guid, List<TfSpacePage>) CreateSpacePage(
		TfSpacePage spacePage);

	public List<TfSpacePage> UpdateSpacePage(
		TfSpacePage spacePage);
	
	public void UpdateSpacePageComponentOptions(
		Guid spacePageId,string optionsJson);	

	public void RenameSpacePage(
		Guid pageId,
		string name);

	public List<TfSpacePage> DeleteSpacePage(
		Guid pageId);

	public (Guid, List<TfSpacePage>) CopySpacePage(
		Guid pageId);

	public void MoveSpacePage(Guid pageId, bool isMoveUp);
	TfSpacePage? GetSpacePageBySpaceViewId(Guid spaceViewId);
}

public partial class TfService : ITfService
{
	public List<TfSpacePage> GetAllSpacePages()
	{
		try
		{
			var spacePagesList = _dboManager.GetList<TfSpacePageDbo>()
				.Select(x => ConvertDboToModel(x))
				.ToList();

			var rootPages = spacePagesList
				.Where(x => x.ParentId is null)
				.OrderBy(x => x.Position)
				.ToList();
			ReadOnlyCollection<TfSpacePageAddonMeta> components = _metaService.GetSpacePagesComponentsMeta();
			foreach (var rootPage in rootPages)
			{
				rootPage.ParentPage = null;
				rootPage.ComponentType = components
					.SingleOrDefault(x => x.ComponentId == rootPage.ComponentId)
					?.Instance.GetType();
				InitSpacePageChildPages(rootPage, spacePagesList, components);
			}

			return rootPages;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public List<TfSpacePage> GetSpacePages(
		Guid spaceId)
	{
		try
		{
			var spacePagesList = _dboManager.GetList<TfSpacePageDbo>(
					spaceId,
					nameof(TfSpacePageDbo.SpaceId))
				.Select(x => ConvertDboToModel(x))
				.ToList();

			var rootPages = spacePagesList
				.Where(x => x.ParentId is null)
				.OrderBy(x => x.Position)
				.ToList();
			var components = _metaService
				.GetSpacePagesComponentsMeta();
			foreach (var rootPage in rootPages)
			{
				rootPage.ParentPage = null;
				rootPage.ComponentType = components
					.SingleOrDefault(x => x.ComponentId == rootPage.ComponentId)
					?.Instance.GetType();
				InitSpacePageChildPages(rootPage, spacePagesList, components);
			}

			return rootPages;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfSpacePage? GetSpacePage(Guid pageId)
	{
		try
		{
			var allPages = GetAllSpacePages();
			return FindPageById(pageId, allPages);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	private void InitSpacePageChildPages(
		TfSpacePage page,
		List<TfSpacePage> allPages,
		ReadOnlyCollection<TfSpacePageAddonMeta> components)
	{
		var childPages = allPages
			.Where(x => x.ParentId == page.Id)
			.OrderBy(x => x.Position)
			.ToList();

		page.ChildPages.AddRange(childPages);

		foreach (var childPage in childPages)
		{
			childPage.ParentPage = page;
			childPage.ComponentType = components
				.SingleOrDefault(x => x.ComponentId == childPage.ComponentId)
				?.Instance.GetType();
			;
			InitSpacePageChildPages(childPage, allPages, components);
		}
	}

	private TfSpacePage? FindPageById(
		Guid id,
		List<TfSpacePage> pages)
	{
		if (pages == null || pages.Count == 0)
			return null;

		Queue<TfSpacePage> queue = new Queue<TfSpacePage>();

		foreach (var page in pages)
		{
			if (page.Id == id)
				return page;

			queue.Enqueue(page);
		}

		while (queue.Count > 0)
		{
			var page = queue.Dequeue();
			if (page.Id == id)
				return page;

			foreach (var childPage in page.ChildPages)
				queue.Enqueue(childPage);
		}

		return null;
	}

	public (Guid, List<TfSpacePage>) CreateSpacePage(
		TfSpacePage spacePage )
	{
		try
		{
			if (spacePage != null && spacePage.Id == Guid.Empty)
				spacePage.Id = Guid.NewGuid();

			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var allPages = GetSpacePages(spacePage.SpaceId);
				var space = GetSpace(spacePage.SpaceId);

				new TfSpacePageValidator(allPages)
					.ValidateCreate(spacePage)
					.ToValidationException()
					.ThrowIfContainsErrors();

				List<TfSpacePage> pagesToUpdate = new List<TfSpacePage>();

				TfSpacePage parentPage = null;
				short? maxPosition = null;
				if (spacePage.ParentId is null)
				{
					maxPosition = allPages
						.Where(x => x.ParentId is null)
						.Select(x => (short?)x.Position.Value)
						.Max(x => x);
				}
				else
				{
					parentPage = FindPageById(spacePage.ParentId.Value, allPages);

					maxPosition = parentPage
						.ChildPages
						.Select(x => (short?)x.Position.Value)
						.Max(x => x);
				}

				if (maxPosition is null)
					maxPosition = 0;

				//if position is not specified or bigger than max position, we add page at the end
				if (spacePage.Position == null || spacePage.Position > maxPosition)
				{
					spacePage.Position = (short)(maxPosition + 1);
				}
				//if new position is equal or lower to 0 and current position is 1, 
				// we do not change position
				else if (spacePage.Position <= 0)
				{
					spacePage.Position = 1;
				}

				var childPagesForPositionUpdate = new List<TfSpacePage>();

				if (parentPage is not null)
				{
					childPagesForPositionUpdate = parentPage.ChildPages
						.Where(x => x.Position.Value >= spacePage.Position)
						.ToList();
				}
				else
				{
					childPagesForPositionUpdate = allPages
						.Where(x => x.ParentId is null && x.Position.Value >= spacePage.Position)
						.ToList();
				}

				foreach (var childPage in childPagesForPositionUpdate)
				{
					childPage.Position++;
					pagesToUpdate.Add(childPage);
				}

				//update pages which position is changed
				foreach (var pageToUpdate in pagesToUpdate)
				{
					if (!_dboManager.Update<TfSpacePageDbo>(ConvertModelToDbo(pageToUpdate),
						    nameof(TfSpacePageDbo.Position)))
						throw new TfDboServiceException("Update<TfSpacePageDbo> failed.");
				}

				if (!_dboManager.Insert<TfSpacePageDbo>(ConvertModelToDbo(spacePage)))
					throw new TfDboServiceException("Insert<TfSpacePageDbo> failed.");


				if (spacePage.Type == TfSpacePageType.Page && spacePage.ComponentId.HasValue)
				{
					var spacePageComponents = _metaService.GetSpacePagesComponentsMeta();
					var pageComponent =
						spacePageComponents.SingleOrDefault(x => x.ComponentId == spacePage.ComponentId);
					if (pageComponent is not null)
					{
						var task = Task.Run(async () =>
						{
							var context = new TfSpacePageAddonContext
							{
								Space = space,
								SpacePage = spacePage,
								TemplateId = spacePage.TemplateId,
								ComponentOptionsJson = spacePage.ComponentOptionsJson,
								Icon = spacePage.FluentIconName,
								Mode = TfComponentMode.Update
							};

							var optionsJson = await pageComponent.Instance.OnPageCreated(_serviceProvider, context);
							if (optionsJson != spacePage.ComponentOptionsJson)
							{
								spacePage.ComponentOptionsJson = optionsJson;
								if (!_dboManager.Update<TfSpacePageDbo>(ConvertModelToDbo(spacePage),
									    nameof(TfSpacePageDbo.ComponentSettingsJson)))
									throw new Exception("Space page update failed");
							}
						});
						task.WaitAndUnwrapException();
					}
				}

				scope.Complete();

				allPages = GetSpacePages(spacePage.SpaceId);
				var createdPage = FindPageById(spacePage.Id, allPages);
				PublishEventWithScope(new TfSpacePageCreatedEvent(createdPage));
				return (spacePage.Id, allPages);
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void RenameSpacePage(
		Guid pageId,
		string name)
	{
		var page = GetSpacePage(pageId);
		if (page is null)
		{
			new ValidationResult(new[]
				{
					new ValidationFailure("",
						"The space page is null.")
				}).ToValidationException()
				.ThrowIfContainsErrors();
		}

		page.Name = name;
		_ = UpdateSpacePage(page);
	}

	public List<TfSpacePage> UpdateSpacePage(
		TfSpacePage spacePage)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var allPages = GetSpacePages(spacePage.SpaceId);
				var space = GetSpace(spacePage.SpaceId);
				new TfSpacePageValidator(allPages)
					.ValidateUpdate(spacePage)
					.ToValidationException()
					.ThrowIfContainsErrors();

				var existingSpacePage = FindPageById(spacePage.Id, allPages);

				List<TfSpacePage> pagesToUpdate = new List<TfSpacePage>();

				pagesToUpdate.Add(spacePage);

				if (spacePage.ParentId != existingSpacePage.ParentId)
				{
					//update existing page parent children pages position
					if (existingSpacePage.ParentId is not null)
					{
						var childPagesForPositionUpdate = new List<TfSpacePage>();

						existingSpacePage.ParentPage.ChildPages.Remove(existingSpacePage);

						childPagesForPositionUpdate = existingSpacePage.ParentPage.ChildPages
							.Where(x => x.Position.Value > existingSpacePage.Position)
							.ToList();

						foreach (var childPage in childPagesForPositionUpdate)
						{
							childPage.Position--;
							if (!pagesToUpdate.Any(x => x.Id == childPage.Id))
								pagesToUpdate.Add(childPage);
						}
					}
					else
					{
						var childPagesForPositionUpdate = new List<TfSpacePage>();

						allPages.Remove(existingSpacePage);

						childPagesForPositionUpdate = allPages
							.Where(x => x.ParentId is null && x.Position.Value > existingSpacePage.Position)
							.ToList();

						foreach (var childPage in childPagesForPositionUpdate)
						{
							childPage.Position--;
							if (!pagesToUpdate.Any(x => x.Id == childPage.Id))
								pagesToUpdate.Add(childPage);
						}
					}

					TfSpacePage parentPage = null;
					short? maxPosition = null;
					if (spacePage.ParentId is null)
					{
						maxPosition = allPages
							.Where(x => x.ParentId is null)
							.Select(x => (short?)x.Position.Value)
							.Max(x => x);
					}
					else
					{
						parentPage = FindPageById(spacePage.ParentId.Value, allPages);

						maxPosition = parentPage
							.ChildPages
							.Select(x => (short?)x.Position.Value)
							.Max(x => x);
					}

					if (maxPosition is null)
						maxPosition = 0;

					//if position is not valid, we add page at the end
					if (spacePage.Position == null || spacePage.Position <= 0 || spacePage.Position > maxPosition)
					{
						spacePage.Position = (short)(maxPosition + 1);
					}
					else
					{
						//else move pages after position we insert at with one position down
						var childPagesForPositionUpdate = new List<TfSpacePage>();

						if (parentPage is null)
						{
							allPages.Remove(existingSpacePage);

							childPagesForPositionUpdate = allPages
								.Where(x => x.Position.Value >= spacePage.Position)
								.ToList();
						}
						else
						{
							parentPage.ChildPages.Remove(existingSpacePage);

							childPagesForPositionUpdate = parentPage.ChildPages
								.Where(x => x.Position.Value >= spacePage.Position)
								.ToList();
						}

						foreach (var childPage in childPagesForPositionUpdate)
						{
							childPage.Position++;
							if (!pagesToUpdate.Any(x => x.Id == childPage.Id))
								pagesToUpdate.Add(childPage);
						}
					}
				}
				else if (spacePage.Position != existingSpacePage.Position)
				{
					TfSpacePage parentPage = null;
					if (spacePage.ParentId is not null)
						parentPage = FindPageById(spacePage.ParentId.Value, allPages);

					var childPagesForPositionDecrease = new List<TfSpacePage>();

					short? maxPosition = null;
					if (spacePage.ParentId is null)
					{
						allPages.Remove(existingSpacePage);

						maxPosition = allPages
							.Where(x => x.ParentId is null)
							.Select(x => (short?)x.Position.Value)
							.Max(x => x);
					}
					else
					{
						parentPage.ChildPages.Remove(existingSpacePage);

						maxPosition = parentPage
							.ChildPages
							.Select(x => (short?)x.Position.Value)
							.Max(x => x);
					}

					if (maxPosition is null)
						maxPosition = 1;

					//if position is not valid, we add page at the end
					if (spacePage.Position == null || spacePage.Position > maxPosition)
					{
						spacePage.Position = (short)(maxPosition);
					}
					//if new position is equal or lower to 0 and current position is 1, 
					// we do not change position
					else if (spacePage.Position <= 0 && existingSpacePage.Position == 1)
					{
						spacePage.Position = 1;
					}

					//only calculate if position is changed
					if (spacePage.Position != existingSpacePage.Position)
					{
						if (parentPage is not null)
						{
							childPagesForPositionDecrease = parentPage.ChildPages
								.Where(x => x.Position.Value >= existingSpacePage.Position)
								.ToList();

							foreach (var childPage in childPagesForPositionDecrease)
							{
								childPage.Position--;
								if (!pagesToUpdate.Any(x => x.Id == childPage.Id))
									pagesToUpdate.Add(childPage);
							}
						}
						else
						{
							childPagesForPositionDecrease = allPages
								.Where(x => x.ParentId is null && x.Position.Value >= existingSpacePage.Position)
								.ToList();

							foreach (var childPage in childPagesForPositionDecrease)
							{
								childPage.Position--;
								if (!pagesToUpdate.Any(x => x.Id == childPage.Id))
									pagesToUpdate.Add(childPage);
							}
						}

						var childPagesForPositionIncrease = new List<TfSpacePage>();

						if (parentPage is not null)
						{
							parentPage.ChildPages.Remove(existingSpacePage);

							childPagesForPositionIncrease = parentPage.ChildPages
								.Where(x => x.Position.Value >= spacePage.Position)
								.ToList();

							foreach (var childPage in childPagesForPositionIncrease)
							{
								childPage.Position++;
								if (!pagesToUpdate.Any(x => x.Id == childPage.Id))
									pagesToUpdate.Add(childPage);
							}
						}
						else
						{
							allPages.Remove(existingSpacePage);

							childPagesForPositionIncrease = allPages
								.Where(x => x.ParentId is null && x.Position.Value >= spacePage.Position)
								.ToList();

							foreach (var childPage in childPagesForPositionIncrease)
							{
								childPage.Position++;
								if (!pagesToUpdate.Any(x => x.Id == childPage.Id))
									pagesToUpdate.Add(childPage);
							}
						}
					}
				}

				//update pages which parent or position is/are changed
				foreach (var childPage in pagesToUpdate)
				{
					if (!_dboManager.Update<TfSpacePageDbo>(ConvertModelToDbo(childPage)))
						throw new TfDboServiceException("Update<TfSpacePageDbo> failed");
				}


				if (spacePage.Type == TfSpacePageType.Page && spacePage.ComponentId.HasValue)
				{
					var spacePageComponents = _metaService.GetSpacePagesComponentsMeta();
					var pageComponent =
						spacePageComponents.SingleOrDefault(x => x.ComponentId == spacePage.ComponentId);
					if (pageComponent is not null)
					{
						var task = Task.Run(async () =>
						{
							var context = new TfSpacePageAddonContext
							{
								Space = space,
								SpacePage = spacePage,
								ComponentOptionsJson = spacePage.ComponentOptionsJson,
								Icon = spacePage.FluentIconName,
								Mode = TfComponentMode.Update
							};

							var optionsJson = await pageComponent.Instance.OnPageUpdated(_serviceProvider, context);
							if (optionsJson != spacePage.ComponentOptionsJson)
							{
								spacePage.ComponentOptionsJson = optionsJson;
								if (!_dboManager.Update<TfSpacePageDbo>(ConvertModelToDbo(spacePage),
									    nameof(TfSpacePageDbo.ComponentSettingsJson)))
									throw new TfDboServiceException("Update<TfSpacePageDbo> failed");
							}
						});
						task.WaitAndUnwrapException();
					}
				}


				scope.Complete();

				allPages = GetSpacePages(spacePage.SpaceId);
				var updatedPage = FindPageById(spacePage.Id, allPages);
				PublishEventWithScope(new TfSpacePageUpdatedEvent(updatedPage));
				return allPages;
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void UpdateSpacePageComponentOptions(
		Guid spacePageId, string optionsJson)
	{
		var spacePage = GetSpacePage(spacePageId);
		if (spacePage is null) throw new ArgumentException(nameof(spacePageId), LOC["Page not found"]);
		if(!optionsJson.StartsWith("{") || !optionsJson.EndsWith("}"))
			throw new ArgumentException(nameof(optionsJson), LOC["Json object is required"]);
		spacePage.ComponentOptionsJson = optionsJson;
		var success = _dboManager.Update<TfSpacePageDbo>(ConvertModelToDbo(spacePage),
			nameof(TfSpacePageDbo.ComponentSettingsJson));
		if (!success)
			throw new TfDboServiceException("Update<TfSpacePageDbo> failed");		
		
		PublishEventWithScope(new TfSpacePageUpdatedEvent(GetSpacePage(spacePageId)!));
	}

	public List<TfSpacePage> DeleteSpacePage(
		Guid pageId)
	{
		try
		{
			var spacePage = GetSpacePage(pageId);
			if (spacePage is null)
				throw new ValidationException("Page not found");			
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{

				var allPages = GetSpacePages(spacePage.SpaceId);
				var space = GetSpace(spacePage.SpaceId);
				new TfSpacePageValidator(allPages)
					.ValidateDelete(spacePage)
					.ToValidationException()
					.ThrowIfContainsErrors();

				TfSpacePage parentPage = null;

				if (spacePage.ParentId.HasValue)
					parentPage = FindPageById(spacePage.ParentId.Value, allPages);

				var childPagesForPositionUpdate = new List<TfSpacePage>();

				if (parentPage is not null)
				{
					childPagesForPositionUpdate = parentPage.ChildPages
						.Where(x => x.Position.Value > spacePage.Position)
						.ToList();
				}
				else
				{
					childPagesForPositionUpdate = allPages
						.Where(x => x.ParentId is null && x.Position.Value > spacePage.Position)
						.ToList();
				}

				foreach (var childPage in childPagesForPositionUpdate)
				{
					childPage.Position--;

					if (!_dboManager.Update<TfSpacePageDbo>(ConvertModelToDbo(childPage),
						    nameof(TfSpacePageDbo.Position)))
						throw new TfDboServiceException("Update<TfSpacePageDbo> failed");
				}

				List<TfSpacePage> pagesToDelete = new List<TfSpacePage>();
				Queue<TfSpacePage> queue = new Queue<TfSpacePage>();

				var page = FindPageById(spacePage.Id, allPages);

				queue.Enqueue(page);

				while (queue.Count > 0)
				{
					var queuePage = queue.Dequeue();
					foreach (var childPage in queuePage.ChildPages)
						queue.Enqueue(childPage);

					pagesToDelete.Add(queuePage);
				}

				pagesToDelete.Reverse();

				var spacePageComponents = _metaService.GetSpacePagesComponentsMeta();

				foreach (var pageToDelete in pagesToDelete)
				{
					var bookmarks = GetBookmarksListForSpacePage(pageToDelete.Id);
					foreach(var bookmark in bookmarks)
						DeleteBookmark(bookmark.Id);

					if (!_dboManager.Delete<TfSpacePageDbo>(pageToDelete.Id))
						throw new TfDboServiceException("Delete<TfSpacePageDbo> failed");

					if (pageToDelete.Type == TfSpacePageType.Page && pageToDelete.ComponentId.HasValue)
					{
						var pageComponent =
							spacePageComponents.SingleOrDefault(x => x.ComponentId == pageToDelete.ComponentId);
						if (pageComponent is not null)
						{
							var task = Task.Run(async () =>
							{
								var context = new TfSpacePageAddonContext
								{
									SpacePage = pageToDelete,
									Space = space,
									ComponentOptionsJson = pageToDelete.ComponentOptionsJson,
									Icon = pageToDelete.FluentIconName,
									Mode = TfComponentMode.Update
								};

								await pageComponent.Instance.OnPageDeleted(_serviceProvider, context);
							});
							task.WaitAndUnwrapException();
						}
					}
				}

				scope.Complete();

				allPages = GetSpacePages(spacePage.SpaceId);
				PublishEventWithScope(new TfSpacePageDeletedEvent(spacePage));
				return allPages;
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public (Guid, List<TfSpacePage>) CopySpacePage(
		Guid pageId)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var allPages = GetAllSpacePages();

				var pageToCopy = FindPageById(pageId, allPages);

				if (pageToCopy is null)
					throw new TfException("Page not found");

				List<TfSpacePage> pagesToCreate = new List<TfSpacePage>();

				Queue<TfSpacePage> queue = new Queue<TfSpacePage>();

				//page is placed after page we copy from
				pageToCopy.Position++;

				queue.Enqueue(pageToCopy);

				while (queue.Count > 0)
				{
					var queuedPage = queue.Dequeue();
					queuedPage.Id = Guid.NewGuid();
					pagesToCreate.Add(queuedPage);

					foreach (var childPage in queuedPage.ChildPages)
					{
						childPage.ParentId = queuedPage.Id;
						queue.Enqueue(childPage);
					}
				}

				foreach (var pageToCreate in pagesToCreate)
				{
					//if page is space view page, remove the space view id from options
					//in order to create a new space view when creating the page
					if (pageToCreate.ComponentId == new Guid(TucSpaceViewSpacePageAddon.Id))
					{
						var options = JsonSerializer.Deserialize<TfSpaceViewSpacePageAddonOptions>(
							pageToCreate.ComponentOptionsJson); 

						if(options.SpaceViewId.HasValue)
						{
							options.SpaceViewId = null;
							pageToCreate.ComponentOptionsJson = JsonSerializer.Serialize(options);
						}
					}

					CreateSpacePage(pageToCreate);
				}

				scope.Complete();

				return (pageToCopy.Id, GetSpacePages(pageToCopy.SpaceId));
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void MoveSpacePage(Guid pageId, bool isMoveUp)
	{
		var page = GetSpacePage(pageId);
		if (page is null)
			throw new ValidationException("Page not found");
		if (isMoveUp)
			page.Position--;
		else
			page.Position++;
		var pageList = UpdateSpacePage(page);
	}

	public TfSpacePage? GetSpacePageBySpaceViewId(Guid spaceViewId)
	{
		var allPages = GetAllSpacePages();
		return allPages.FirstOrDefault(x => x.ComponentOptionsJson.Contains(spaceViewId.ToString()));
	}

	private TfSpacePage ConvertDboToModel(
		TfSpacePageDbo dbo)
	{
		if (dbo == null)
			return null;

		return new TfSpacePage
		{
			Id = dbo.Id,
			Name = dbo.Name,
			Position = dbo.Position,
			SpaceId = dbo.SpaceId,
			Type = dbo.Type,
			ComponentOptionsJson = dbo.ComponentSettingsJson,
			ComponentId = dbo.ComponentId,
			FluentIconName = dbo.Icon,
			ParentId = dbo.ParentId
		};
	}

	private TfSpacePageDbo ConvertModelToDbo(
		TfSpacePage model)
	{
		if (model == null)
			return null;

		return new TfSpacePageDbo
		{
			Id = model.Id,
			Name = model.Name,
			Position = model.Position ?? 0,
			SpaceId = model.SpaceId,
			Type = model.Type,
			ParentId = model.ParentId,
			Icon = model.FluentIconName ?? string.Empty,
			ComponentId = model.ComponentId,
			ComponentSettingsJson = model.ComponentOptionsJson ?? "{}",
		};
	}

	#region <--- validation --->

	internal class TfSpacePageValidator
		: AbstractValidator<TfSpacePage>
	{
		private readonly List<TfSpacePage> _allPages;

		public TfSpacePageValidator(
			List<TfSpacePage> allPages)
		{
			_allPages = allPages;

			RuleSet("general", () =>
			{
				RuleFor(spacePage => spacePage.Id)
					.NotEmpty()
					.WithMessage("The space page id is required.");

				RuleFor(spacePage => spacePage.Name)
					.NotEmpty()
					.WithMessage("The space page name is required.");

				RuleFor(spacePage => spacePage.SpaceId)
					.NotEmpty()
					.WithMessage("The space id is required.");
			});

			RuleSet("create", () =>
			{
				RuleFor(spacePage => spacePage.Id)
					.Must((spacePage, id) => { return FindPageById(id, allPages) == null; })
					.WithMessage("There is already existing space page with specified identifier.");

				RuleFor(spacePage => spacePage.ParentId)
					.Must((spacePage, parentId) => { return spacePage.Id != parentId; })
					.WithMessage("Space page cannot be parent to itself.");
			});

			RuleSet("update", () =>
			{
				RuleFor(spacePage => spacePage.Id)
					.Must((spacePage, id) => { return FindPageById(id, allPages) != null; })
					.WithMessage("There is not existing space page with specified identifier.");

				RuleFor(spacePage => spacePage.ParentId)
					.Must((spacePage, parentId) => { return spacePage.Id != parentId; })
					.WithMessage("Space page cannot be parent to itself.");

				RuleFor(spacePage => spacePage.ParentId)
					.Must((spacePage, parentId) =>
					{
						var existingPage = FindPageById(spacePage.Id, allPages);

						return existingPage
							.GetChildPagesPlainList()
							.Count(x => x.ParentId == parentId) == 0;
					})
					.WithMessage("Space page cannot be moved inside its child tree.");
			});

			RuleSet("delete", () =>
			{
			});
		}

		public ValidationResult ValidateCreate(
			TfSpacePage spacePage)
		{
			if (spacePage == null)
				return new ValidationResult(new[]
				{
					new ValidationFailure("",
						"The space page is null.")
				});

			return this.Validate(spacePage, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(
			TfSpacePage spacePage)
		{
			if (spacePage == null)
				return new ValidationResult(new[]
				{
					new ValidationFailure("",
						"The space page is null.")
				});

			return this.Validate(spacePage, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}

		public ValidationResult ValidateDelete(
			TfSpacePage spacePage)
		{
			if (spacePage == null)
				return new ValidationResult(new[]
				{
					new ValidationFailure("",
						"The space page is null.")
				});

			return this.Validate(spacePage, options =>
			{
				options.IncludeRuleSets("delete");
			});
		}

		private TfSpacePage FindPageById(
			Guid id,
			List<TfSpacePage> pages)
		{
			if (pages == null || pages.Count == 0)
				return null;

			Queue<TfSpacePage> queue = new Queue<TfSpacePage>();

			foreach (var page in pages)
			{
				if (page.Id == id)
					return page;

				queue.Enqueue(page);
			}

			while (queue.Count > 0)
			{
				var page = queue.Dequeue();
				if (page.Id == id)
					return page;

				foreach (var childPage in page.ChildPages)
					queue.Enqueue(childPage);
			}

			return null;
		}
	}

	#endregion
}