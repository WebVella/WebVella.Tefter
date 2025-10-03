using System.Text.Json.Serialization.Metadata;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	public List<TfSpaceView> GetAllSpaceViews(string? search = null);

	public List<TfSpaceView> GetSpaceViewsList(
		Guid spaceId, string? search = null);

	public TfSpaceView GetSpaceView(
		Guid id);

	public Task<TfSpaceView> CreateSpaceView(
		TfSpaceViewCreateModel spaceViewExt);

	public TfSpaceView CreateSpaceView(
		TfSpaceView spaceView);

	public TfSpaceView UpdateSpaceView(
		TfSpaceView spaceView);

	public Task UpdateSpaceViewSettings(
		Guid spaceViewId,
		TfSpaceViewSettings settings);

	public Task UpdateSpaceViewPresets(
		Guid spaceViewId,
		List<TfSpaceViewPreset> presets);

	public void DeleteSpaceView(
		Guid id);

}

public partial class TfService : ITfService
{
	public List<TfSpaceView> GetAllSpaceViews(string? search = null)
	{
		try
		{
			var spaceViewsDbo = _dboManager.GetList<TfSpaceViewDbo>();
			var allSpaceViews = spaceViewsDbo.Select(x => ConvertDboToModel(x)).ToList();
			var spaceDataDict = (GetDatasets() ?? new List<TfDataset>()).ToDictionary(x => x.Id);
			foreach (var spaceView in allSpaceViews)
			{
				if (spaceDataDict.ContainsKey(spaceView.DatasetId))
					spaceView.SpaceDataName = spaceDataDict[spaceView.DatasetId].Name;
			}

			if (String.IsNullOrWhiteSpace(search))
				return allSpaceViews;
			search = search.Trim().ToLowerInvariant();
			return allSpaceViews.Where(x =>
				x.Name.ToLowerInvariant().Contains(search)
				).ToList();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public List<TfSpaceView> GetSpaceViewsList(
		Guid spaceId, string? search = null)
	{
		try
		{
			var orderSettings = new TfOrderSettings(
			nameof(TfSpace.Position),
			OrderDirection.ASC);

			var spaceViews = _dboManager.GetList<TfSpaceViewDbo>(
				spaceId,
				nameof(TfSpaceView.SpaceId),
				order: orderSettings);

			var spaceDataDict = GetDatasets().ToDictionary(x => x.Id);
			var allSpaceViews = spaceViews.Select(x => ConvertDboToModel(x)).ToList();

			foreach (var spaceView in allSpaceViews)
			{
				if (spaceDataDict.ContainsKey(spaceView.DatasetId))
					spaceView.SpaceDataName = spaceDataDict[spaceView.DatasetId].Name;
			}
			if (String.IsNullOrWhiteSpace(search))
				return allSpaceViews;
			search = search.Trim().ToLowerInvariant();
			return allSpaceViews.Where(x =>
				x.Name.ToLowerInvariant().Contains(search)
				).ToList();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}


	public TfSpaceView GetSpaceView(
		Guid id)
	{
		try
		{
			var spaceView = _dboManager.Get<TfSpaceViewDbo>(id);
			return ConvertDboToModel(spaceView);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public async Task<TfSpaceView> CreateSpaceView(
		TfSpaceViewCreateModel spaceViewExt)
	{
		try
		{
			if (spaceViewExt is null)
				throw new ArgumentException(nameof(spaceViewExt));
			if (spaceViewExt != null && spaceViewExt.Id == Guid.Empty)
				spaceViewExt.Id = Guid.NewGuid();

			TfDataset? dataset = null;
			TfSpaceView? spaceView = null;
			#region << Validate>>

			var valEx = new TfValidationException();

			//args
			if (String.IsNullOrWhiteSpace(spaceViewExt.Name))
				valEx.AddValidationError(nameof(spaceViewExt.Name), "required");

			if (spaceViewExt.DatasetId is null)
				valEx.AddValidationError(nameof(spaceViewExt.DatasetId), "required");
			else
			{
				dataset = GetDataset(spaceViewExt.DatasetId.Value);
				if (dataset is null)
					valEx.AddValidationError(nameof(spaceViewExt.DatasetId), "dataset is not found");
			}
			valEx.ThrowIfContainsErrors();

			#endregion

			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var columNameDbTypeDict = new Dictionary<string, TfDatabaseColumnType>();
				var providers = GetDataProviders();
				foreach (var dp in providers)
				{
					foreach (var column in dp.Columns)
					{
						columNameDbTypeDict[column.DbName!] = column.DbType;
					}
				}
				foreach (var column in GetSharedColumns())
				{
					columNameDbTypeDict[column.DbName!] = column.DbType;
				}

				#region << create view>>
				{
					var spaceViewObj = new TfSpaceView()
					{
						Id = Guid.NewGuid(),
						Name = spaceViewExt.Name,
						Position = 1,//will be overrided later
						DatasetId = dataset.Id,
						Presets = spaceViewExt.Presets,
						SettingsJson = JsonSerializer.Serialize(spaceViewExt.Settings),
					};

					spaceView = CreateSpaceView(spaceViewObj);
				}
				#endregion

				#region << create view columns>>
				{
					var availableTypes = GetAvailableSpaceViewColumnTypes();
					var columnsToCreate = new List<TfSpaceViewColumn>();
					short position = 1;


					var columnsList = new List<string>();
					columnsList.AddRange(dataset.Columns);
					foreach (var identity in dataset.Identities)
					{
						foreach (var column in identity.Columns)
						{
							columnsList.Add($"{identity.DataIdentity}.{column}");
						}
					}

					//if no columns are found in the dataset add all provider columns
					if (columnsList.Count == 0)
					{
						var datasetProvider = providers.Single(x=> x.Id == dataset.DataProviderId);
						datasetProvider.Columns.ToList().ForEach(x=> columnsList.Add(x.DbName!));
					}


					foreach (var columnName in columnsList)
					{

						TfDatabaseColumnType? dbType = null;
						if (!columnName.Contains("."))
						{
							dbType = columNameDbTypeDict.ContainsKey(columnName) ? columNameDbTypeDict[columnName] : null;
						}
						else
						{
							var columnArray = columnName.Split('.');
							dbType = columNameDbTypeDict.ContainsKey(columnArray[1]) ? columNameDbTypeDict[columnArray[1]] : null;
						}
						if (dbType is null) continue;

						var columnType = ModelHelpers.GetColumnTypeForDbType(dbType.Value, availableTypes);
						var tfColumn = new TfSpaceViewColumn
						{
							Id = Guid.NewGuid(),
							SpaceViewId = spaceView.Id,
							Position = position,
							Title = columnName,
							QueryName = columnName,
							ComponentOptionsJson = "{}",
							EditComponentOptionsJson = "{}",
							DataMapping = new(),
							TypeId = Guid.Empty,
							ComponentId = Guid.Empty
						};

						if (columnType is not null)
						{
							tfColumn.TypeId = columnType.AddonId;
							tfColumn.ComponentId = columnType.DefaultDisplayComponentId ?? new Guid(TucTextDisplayColumnComponent.ID);
							tfColumn.EditComponentId = columnType.DefaultEditComponentId ?? new Guid(TucTextEditColumnComponent.ID);
							foreach (var mapper in columnType.DataMapping)
							{
								tfColumn.DataMapping[mapper.Alias] = columnName;
							}
						}
						columnsToCreate.Add(tfColumn);
						position++;
					}

					foreach (var tfColumn in columnsToCreate)
					{
						var createdColumn = await CreateSpaceViewColumn(tfColumn);
						if (createdColumn is null)
							throw new TfException("CreateSpaceViewColumn failed to return newly created object");
					}
				}
				#endregion

				scope.Complete();

				return GetSpaceView(spaceView.Id);
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfSpaceView CreateSpaceView(
		TfSpaceView spaceView)
	{
		try
		{
			if (spaceView != null && spaceView.Id == Guid.Empty)
				spaceView.Id = Guid.NewGuid();

			//TODO RUMEN -> As Space Id will be removed from the view it is hardcoded here but needs to be removed
			{
				spaceView.SpaceId = GetSpacesList().First().Id;
			}


			new TfSpaceViewValidator(this)
				.ValidateCreate(spaceView)
				.ToValidationException()
				.ThrowIfContainsErrors();

			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var spaceViews = GetSpaceViewsList(spaceView.SpaceId);

				//position is ignored - space is added at last place
				spaceView.Position = (short)(spaceViews.Count + 1);

				var success = _dboManager.Insert<TfSpaceViewDbo>(ConvertModelToDbo(spaceView));
				if (!success)
					throw new TfDboServiceException("Insert<TfSpaceViewDbo> failed.");

				scope.Complete();

				return GetSpaceView(spaceView.Id);
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public async Task UpdateSpaceViewSettings(
		Guid spaceViewId,
		TfSpaceViewSettings settings)
	{
		try
		{
			var existingSpaceView = _dboManager.Get<TfSpaceViewDbo>(spaceViewId);
			if (existingSpaceView == null)
			{
				throw new TfValidationException("spaceViewId", "SpaceView not found.");
			}

			existingSpaceView.SettingsJson = JsonSerializer.Serialize(settings);

			var success = _dboManager.Update<TfSpaceViewDbo>(
				existingSpaceView,
				nameof(TfSpaceViewDbo.SettingsJson));

			if (!success)
				throw new TfDboServiceException("Update<TfSpaceViewDbo> failed.");
			
			var spaceView = GetSpaceView(spaceViewId);
			await PublishEventWithScopeAsync(new TfSpaceViewUpdatedEvent(spaceView) );			
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}
	
	
	public async Task UpdateSpaceViewPresets(
		Guid spaceViewId,
		List<TfSpaceViewPreset> presets)
	{
		try
		{
			var existingSpaceView = _dboManager.Get<TfSpaceViewDbo>(spaceViewId);
			if (existingSpaceView == null)
			{
				throw new TfValidationException("spaceViewId", "SpaceView not found.");
			}

			var jsonOptions = new JsonSerializerOptions
			{
				TypeInfoResolver = new DefaultJsonTypeInfoResolver
				{
					Modifiers = { JsonExtensions.AddPrivateProperties<JsonIncludePrivatePropertyAttribute>() },
				},
			};

			existingSpaceView.PresetsJson = JsonSerializer.Serialize(presets ?? new List<TfSpaceViewPreset>(), jsonOptions);

			var success = _dboManager.Update<TfSpaceViewDbo>(
				existingSpaceView,
				nameof(TfSpaceViewDbo.PresetsJson));

			if (!success)
				throw new TfDboServiceException("Update<TfSpaceViewDbo> failed.");
			
			var spaceView = GetSpaceView(spaceViewId);
			await PublishEventWithScopeAsync(new TfSpaceViewUpdatedEvent(spaceView) );				
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfSpaceView UpdateSpaceView(
	TfSpaceView spaceView)
	{
		try
		{
			new TfSpaceViewValidator(this)
			.ValidateUpdate(spaceView)
			.ToValidationException()
			.ThrowIfContainsErrors();

			var existingSpaceView = _dboManager.Get<TfSpaceViewDbo>(spaceView.Id);

			//position is not updated
			spaceView.Position = existingSpaceView.Position;

			var success = _dboManager.Update<TfSpaceViewDbo>(ConvertModelToDbo(spaceView));

			if (!success)
				throw new TfDboServiceException("Update<TfSpaceViewDbo> failed.");

			return GetSpaceView(spaceView.Id);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void DeleteSpaceView(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var spaceView = GetSpaceView(id);

				new TfSpaceViewValidator(this)
					.ValidateDelete(spaceView)
					.ToValidationException()
					.ThrowIfContainsErrors();

				bool success = false;

				var bookmarks = GetBookmarksListForSpaceView(id);
				foreach (var bookmark in bookmarks)
				{
					DeleteBookmark(bookmark.Id);
				}

				var spacesAfter = GetSpaceViewsList(spaceView.SpaceId)
					.Where(x => x.Position > spaceView.Position).ToList();

				//update positions for spaces after the one being deleted
				foreach (var spaceAfter in spacesAfter)
				{
					spaceAfter.Position--;

					success = _dboManager.Update<TfSpaceViewDbo>(ConvertModelToDbo(spaceAfter));
					if (!success)
						throw new TfDboServiceException("Delete<TfSpaceViewDbo> failed");
				}

				var spaceViewColumns = GetSpaceViewColumnsList(spaceView.Id);
				foreach (var column in spaceViewColumns)
				{
					DeleteSpaceViewColumn(column.Id);
				}

				success = _dboManager.Delete<TfSpaceViewDbo>(id);
				if (!success)
					throw new TfDboServiceException("Delete<TfSpaceViewDbo> failed");

				scope.Complete();
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	private TfSpaceView ConvertDboToModel(
		TfSpaceViewDbo dbo)
	{
		if (dbo == null)
			return null;

		List<TfSpaceViewPreset> presets = new List<TfSpaceViewPreset>();

		if (!string.IsNullOrWhiteSpace(dbo.PresetsJson))
		{
			var jsonOptions = new JsonSerializerOptions
			{
				TypeInfoResolver = new DefaultJsonTypeInfoResolver
				{
					Modifiers = { JsonExtensions.AddPrivateProperties<JsonIncludePrivatePropertyAttribute>() },
				},
			};
			presets = JsonSerializer.Deserialize<List<TfSpaceViewPreset>>(dbo.PresetsJson, jsonOptions);
		}

		return new TfSpaceView
		{
			Id = dbo.Id,
			Name = dbo.Name,
			Position = dbo.Position,
			SettingsJson = dbo.SettingsJson,
			DatasetId = dbo.SpaceDataId,
			SpaceId = dbo.SpaceId,
			Type = dbo.Type,
			Presets = presets
		};

	}

	private TfSpaceViewDbo ConvertModelToDbo(
		TfSpaceView model)
	{
		if (model == null)
			return null;

		return new TfSpaceViewDbo
		{
			Id = model.Id,
			Name = model.Name,
			Position = model.Position,
			SettingsJson = model.SettingsJson,
			SpaceDataId = model.DatasetId,
			SpaceId = model.SpaceId,
			Type = model.Type,
			PresetsJson = JsonSerializer.Serialize(model.Presets ?? new List<TfSpaceViewPreset>())
		};
	}

	#region <--- validation --->

	internal class TfSpaceViewValidator
	: AbstractValidator<TfSpaceView>
	{
		public TfSpaceViewValidator(
			ITfService tfService)
		{

			RuleSet("general", () =>
			{
				RuleFor(spaceView => spaceView.Id)
					.NotEmpty()
					.WithMessage("The space view id is required.");

				RuleFor(spaceView => spaceView.Name)
					.NotEmpty()
					.WithMessage("The space view name is required.");

				RuleFor(spaceView => spaceView.SpaceId)
					.NotEmpty()
					.WithMessage("The space id is required.");

				RuleFor(spaceView => spaceView.DatasetId)
					.NotEmpty()
					.WithMessage("The space data id is required.");

				//TODO rumen more validation about space data - SpaceId and SpaceId in space view

			});

			RuleSet("create", () =>
			{
				RuleFor(spaceView => spaceView.Id)
						.Must((spaceView, id) => { return tfService.GetSpaceView(id) == null; })
						.WithMessage("There is already existing space view with specified identifier.");

			});

			RuleSet("update", () =>
			{
				RuleFor(spaceView => spaceView.Id)
						.Must((spaceView, id) =>
						{
							return tfService.GetSpaceView(id) != null;
						})
						.WithMessage("There is not existing space view with specified identifier.");
			});

			RuleSet("delete", () =>
			{
			});

		}

		public ValidationResult ValidateCreate(
			TfSpaceView spaceView)
		{
			if (spaceView == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The space view is null.") });

			return this.Validate(spaceView, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}


		public ValidationResult ValidateUpdate(
			TfSpaceView spaceView)
		{
			if (spaceView == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The space view is null.") });

			return this.Validate(spaceView, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}

		public ValidationResult ValidateDelete(
			TfSpaceView spaceView)
		{
			if (spaceView == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The space view with specified identifier is not found.") });

			return this.Validate(spaceView, options =>
			{
				options.IncludeRuleSets("delete");
			});
		}

	}

	#endregion

}
