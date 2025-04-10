namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	public List<TfSpace> GetSpacesList();

	public List<TfSpace> GetSpacesListForUser(
		Guid userId);

	public TfSpace GetSpace(
		Guid id);

	public TfSpace CreateSpace(
		TfSpace space);

	public TfSpace UpdateSpace(
		TfSpace space);

	public void DeleteSpace(
		Guid id);

	public void MoveSpaceUp(
		Guid id);

	public void MoveSpaceDown(
		Guid id);
}

public partial class TfService : ITfService
{
	public List<TfSpace> GetSpacesList()
	{
		try
		{
			var orderSettings = new TfOrderSettings(
				nameof(TfSpace.Position),
				OrderDirection.ASC);

			var dbos = _dboManager.GetList<TfSpaceDbo>(
				order: orderSettings);

			return dbos.Select(x => ConvertDboToModel(x)).ToList();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	/// <summary>
	/// This is a placeholder method. In the future there will be private spaces with limited access
	/// and the user will not be able to access all of them, so spaces from the UI should be get with the userId
	/// </summary>
	/// <param name="userId"></param>
	/// <returns></returns>
	public List<TfSpace> GetSpacesListForUser(Guid userId) => GetSpacesList();

	public TfSpace GetSpace(
		Guid id)
	{
		try
		{
			var dbo = _dboManager.Get<TfSpaceDbo>(id);
			return ConvertDboToModel(dbo);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfSpace CreateSpace(
		TfSpace space)
	{
		try
		{
			if (space != null && space.Id == Guid.Empty)
				space.Id = Guid.NewGuid();

			new TfSpaceValidator(this)
				.ValidateCreate(space)
				.ToValidationException()
				.ThrowIfContainsErrors();


			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var spaces = GetSpacesList();

				//position is ignored - space is added at last place
				var dbo = ConvertModelToDbo(space);
				dbo.Position = (short)(spaces.Count + 1);

				var success = _dboManager.Insert<TfSpaceDbo>(dbo);
				if (!success)
					throw new TfDboServiceException("Insert<TfSpaceDbo> failed");

				scope.Complete();

				return GetSpace(space.Id);
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfSpace UpdateSpace(
		TfSpace space)
	{
		try
		{
			new TfSpaceValidator(this)
				.ValidateUpdate(space)
				.ToValidationException()
				.ThrowIfContainsErrors();

			var existingSpace = _dboManager.Get<TfSpaceDbo>(space.Id);

			//position is not updated
			var dbo = ConvertModelToDbo(space);
			dbo.Position = existingSpace.Position;

			var success = _dboManager.Update<TfSpaceDbo>(dbo);
			if (!success)
				throw new TfDboServiceException("Update<TfSpaceDbo> failed");

			return GetSpace(space.Id);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void MoveSpaceUp(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var spaces = GetSpacesList();

				var space = spaces.SingleOrDefault(x => x.Id == id);

				if (space == null)
				{
					throw new TfValidationException(nameof(id),
						"Found no space for specified identifier.");
				}

				if (space.Position == 1)
					return;

				var prevSpace = spaces.SingleOrDefault(x => x.Position == (space.Position - 1));
				space.Position = (short)(space.Position - 1);

				if (prevSpace != null)
					prevSpace.Position = (short)(prevSpace.Position + 1);

				var success = _dboManager.Update<TfSpaceDbo>(ConvertModelToDbo(space));
				if (!success)
					throw new TfDboServiceException("Update<TfSpaceDbo> failed");

				if (prevSpace != null)
				{
					success = _dboManager.Update<TfSpaceDbo>(ConvertModelToDbo(prevSpace));
					if (!success)
						throw new TfDboServiceException("Update<TfSpaceDbo> failed");
				}

				scope.Complete();
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void MoveSpaceDown(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var spaces = GetSpacesList();

				var space = spaces.SingleOrDefault(x => x.Id == id);

				if (space == null)
				{
					throw new TfValidationException(nameof(id),
						"Found no space for specified identifier.");
				}

				if (space.Position == spaces.Count)
					return;


				var nextSpace = spaces.SingleOrDefault(x => x.Position == (space.Position + 1));
				space.Position = (short)(space.Position + 1);

				if (nextSpace != null)
					nextSpace.Position = (short)(nextSpace.Position - 1);

				var success = _dboManager.Update<TfSpaceDbo>(ConvertModelToDbo(space));
				if (!success)
					throw new TfDboServiceException("Update<TfSpaceDbo> failed");

				if (nextSpace != null)
				{
					success = _dboManager.Update<TfSpaceDbo>(ConvertModelToDbo(nextSpace));
					if (!success)
						throw new TfDboServiceException("Update<TfSpaceDbo> failed");
				}

				scope.Complete();
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void DeleteSpace(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var space = GetSpace(id);

				new TfSpaceValidator(this)
					.ValidateDelete(space)
					.ToValidationException()
					.ThrowIfContainsErrors();

				var spaceViews = GetSpaceViewsList(space.Id);
				foreach (var spaceView in spaceViews)
				{
					DeleteSpaceView(spaceView.Id);
				}

				var spaceDataList = GetSpaceDataList(id);

				foreach (var spaceData in spaceDataList)
				{
					DeleteSpaceData(spaceData.Id);
				}

				var spacePages = GetSpacePages(id);
				foreach (var spacePage in spacePages.OrderByDescending(x => x.Position))
				{
					DeleteSpacePage(spacePage);
				}

				var spacesAfter = GetSpacesList()
					.Where(x => x.Position > space.Position)
					.ToList();

				//update positions for spaces after the one being deleted
				foreach (var spaceAfter in spacesAfter)
				{
					spaceAfter.Position--;

					var successUpdatePosition = _dboManager.Update<TfSpaceDbo>(ConvertModelToDbo(spaceAfter));
					if (!successUpdatePosition)
						throw new TfDboServiceException("Update<TfSpaceDbo> failed");
				}

				var success = _dboManager.Delete<TfSpaceDbo>(id);

				if (!success)
					throw new TfDboServiceException("Delete<TfSpaceDbo> failed");

				scope.Complete();
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	private TfSpace ConvertDboToModel(
		TfSpaceDbo dbo)
	{
		if (dbo == null)
			return null;

		return new TfSpace
		{
			Id = dbo.Id,
			Name = dbo.Name,
			IsPrivate = dbo.IsPrivate,
			Position = dbo.Position,
			Color = dbo.Color,
			Icon = dbo.Icon,
		};

	}

	private TfSpaceDbo ConvertModelToDbo(
		TfSpace model)
	{
		if (model == null)
			return null;

		return new TfSpaceDbo
		{
			Id = model.Id,
			Name = model.Name,
			IsPrivate = model.IsPrivate,
			Position = model.Position,
			Color = model.Color,
			Icon = model.Icon,
		};
	}


	#region <--- validation --->

	internal class TfSpaceValidator
	: AbstractValidator<TfSpace>
	{
		public TfSpaceValidator(
			ITfService tfService)
		{

			RuleSet("general", () =>
			{
				RuleFor(space => space.Id)
					.NotEmpty()
					.WithMessage("The space id is required.");

				RuleFor(space => space.Name)
					.NotEmpty()
					.WithMessage("The space name is required.");

			});

			RuleSet("create", () =>
			{
				RuleFor(space => space.Id)
						.Must((space, id) => { return tfService.GetSpace(id) == null; })
						.WithMessage("There is already existing space with specified identifier.");

				RuleFor(space => space.Name)
						.Must((space, name) =>
						{
							if (string.IsNullOrEmpty(name))
								return true;

							var spaces = tfService.GetSpacesList();
							return !spaces.Any(x => x.Name.ToLowerInvariant().Trim() == name.ToLowerInvariant().Trim());
						})
						.WithMessage("There is already existing space with same name.");
			});

			RuleSet("update", () =>
			{
				RuleFor(space => space.Id)
						.Must((space, id) =>
						{
							return tfService.GetSpace(id) != null;
						})
						.WithMessage("There is not existing space with specified identifier.");

			});

			RuleSet("delete", () =>
			{
			});

		}

		public ValidationResult ValidateCreate(
			TfSpace space)
		{
			if (space == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The space is null.") });

			return this.Validate(space, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(
			TfSpace space)
		{
			if (space == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The space is null.") });

			return this.Validate(space, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}

		public ValidationResult ValidateDelete(
			TfSpace space)
		{
			if (space == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The space with specified identifier is not found.") });

			return this.Validate(space, options =>
			{
				options.IncludeRuleSets("delete");
			});
		}
	}

	#endregion
}
