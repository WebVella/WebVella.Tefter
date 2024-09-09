namespace WebVella.Tefter;

public partial interface ITfSpaceManager
{

	public Result<List<TfSpaceView>> GetAllSpaceViews();
	public Result<List<TfSpaceView>> GetSpaceViewsList(
		Guid spaceId);

	public Result<TfSpaceView> GetSpaceView(
		Guid id);

	public Result<TfSpaceView> CreateSpaceView(
		TfCreateSpaceViewExtended spaceViewExt);

	public Result<TfSpaceView> CreateSpaceView(
		TfSpaceView spaceView);

	public Result<TfSpaceView> UpdateSpaceView(
		TfSpaceView spaceView);

	public Result DeleteSpaceView(
		Guid id);

	public Result MoveSpaceViewUp(
		Guid id);

	public Result MoveSpaceViewDown(
		Guid id);
}

public partial class TfSpaceManager : ITfSpaceManager
{
	public Result<List<TfSpaceView>> GetAllSpaceViews()
	{
		try
		{
			var spaceViews = _dboManager.GetList<TfSpaceView>();

			return Result.Ok(spaceViews);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get list of space views").CausedBy(ex));
		}
	}
	public Result<List<TfSpaceView>> GetSpaceViewsList(
		Guid spaceId)
	{
		try
		{
			var orderSettings = new OrderSettings(
				nameof(TfSpace.Position),
				OrderDirection.ASC);

			var spaceViews = _dboManager.GetList<TfSpaceView>(
				spaceId,
				nameof(TfSpaceView.SpaceId),
				order: orderSettings);

			return Result.Ok(spaceViews);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get list of space views").CausedBy(ex));
		}
	}


	public Result<TfSpaceView> GetSpaceView(
		Guid id)
	{
		try
		{
			var spaceView = _dboManager.Get<TfSpaceView>(id);
			return Result.Ok(spaceView);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get space view by id").CausedBy(ex));
		}
	}

	public Result<TfSpaceView> CreateSpaceView(
		TfCreateSpaceViewExtended spaceViewExt)
	{
		try
		{
			if (spaceViewExt != null && spaceViewExt.Id == Guid.Empty)
				spaceViewExt.Id = Guid.NewGuid();

			throw new NotImplementedException();
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create space view").CausedBy(ex));
		}
	}

	public Result<TfSpaceView> CreateSpaceView(
		TfSpaceView spaceView)
	{
		try
		{
			if (spaceView != null && spaceView.Id == Guid.Empty)
				spaceView.Id = Guid.NewGuid();

			TfSpaceViewValidator validator =
				new TfSpaceViewValidator(_dboManager, this);

			var validationResult = validator.ValidateCreate(spaceView);

			if (!validationResult.IsValid)
				return validationResult.ToResult();

			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var spaceViews = GetSpaceViewsList(spaceView.SpaceId).Value;

				//position is ignored - space is added at last place
				spaceView.Position = (short)(spaceViews.Count + 1);

				var success = _dboManager.Insert<TfSpaceView>(spaceView);

				if (!success)
					return Result.Fail(new DboManagerError("Insert", spaceView));

				scope.Complete();

				return GetSpaceView(spaceView.Id);
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create space view").CausedBy(ex));
		}
	}

	public Result<TfSpaceView> UpdateSpaceView(
		TfSpaceView spaceView)
	{
		try
		{
			TfSpaceViewValidator validator =
				new TfSpaceViewValidator(_dboManager, this);

			var validationResult = validator.ValidateUpdate(spaceView);

			if (!validationResult.IsValid)
				return validationResult.ToResult();

			var existingSpaceView = _dboManager.Get<TfSpaceView>(spaceView.Id);

			//position is not updated
			spaceView.Position = existingSpaceView.Position;

			var success = _dboManager.Update<TfSpaceView>(spaceView);

			if (!success)
				return Result.Fail(new DboManagerError("Update", spaceView));

			return GetSpaceView(spaceView.Id);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to update space").CausedBy(ex));
		}

	}

	public Result MoveSpaceViewUp(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var spaceView = GetSpaceView(id).Value;

				if (spaceView == null)
					return Result.Fail(new ValidationError(
						nameof(id),
						"Found no space view for specified identifier."));

				if (spaceView.Position == 1)
					return Result.Ok();

				var spaceViews = GetSpaceViewsList(spaceView.SpaceId).Value;

				var prevSpace = spaceViews.SingleOrDefault(x => x.Position == (spaceView.Position - 1));
				spaceView.Position = (short)(spaceView.Position - 1);

				if (prevSpace != null)
					prevSpace.Position = (short)(prevSpace.Position + 1);

				var success = _dboManager.Update<TfSpaceView>(spaceView);

				if (!success)
					return Result.Fail(new DboManagerError("Update", spaceView));

				if (prevSpace != null)
				{
					success = _dboManager.Update<TfSpaceView>(prevSpace);

					if (!success)
						return Result.Fail(new DboManagerError("Update", prevSpace));
				}

				scope.Complete();

				return Result.Ok();
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to move space view up in position").CausedBy(ex));
		}
	}

	public Result MoveSpaceViewDown(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{


				var spaceView = GetSpaceView(id).Value;

				if (spaceView == null)
					return Result.Fail(new ValidationError(
						nameof(id),
						"Found no space for specified identifier."));

				var spaceViews = GetSpaceViewsList(spaceView.SpaceId).Value;

				if (spaceView.Position == spaceViews.Count)
					return Result.Ok();

				var nextSpaceView = spaceViews.SingleOrDefault(x => x.Position == (spaceView.Position + 1));
				spaceView.Position = (short)(spaceView.Position + 1);

				if (nextSpaceView != null)
					nextSpaceView.Position = (short)(nextSpaceView.Position - 1);

				var success = _dboManager.Update<TfSpaceView>(spaceView);

				if (!success)
					return Result.Fail(new DboManagerError("Update", spaceView));

				if (nextSpaceView != null)
				{
					success = _dboManager.Update<TfSpaceView>(nextSpaceView);

					if (!success)
						return Result.Fail(new DboManagerError("Update", nextSpaceView));
				}

				scope.Complete();

				return Result.Ok();
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to move space down in position").CausedBy(ex));
		}
	}

	public Result DeleteSpaceView(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				TfSpaceViewValidator validator =
					new TfSpaceViewValidator(_dboManager, this);

				var spaceView = GetSpaceView(id).Value;

				var validationResult = validator.ValidateDelete(spaceView);

				if (!validationResult.IsValid)
					return validationResult.ToResult();


				var spacesAfter = GetSpaceViewsList(spaceView.SpaceId)
					.Value
					.Where(x => x.Position > spaceView.Position).ToList();

				//update positions for spaces after the one being deleted
				foreach (var spaceAfter in spacesAfter)
				{
					spaceAfter.Position--;
					var successUpdatePosition = _dboManager.Update<TfSpaceView>(spaceAfter);

					if (!successUpdatePosition)
						return Result.Fail(new DboManagerError("Failed to update space view position" +
							" during delete space process", spaceAfter));
				}

				var spaceViewColumns = GetSpaceViewColumnsList(spaceView.Id).Value;

				foreach(var column in spaceViewColumns )
				{
					var columnDeleteResult = DeleteSpaceViewColumn(column.Id);

					if (!columnDeleteResult.IsSuccess)
						return Result.Fail(new DboManagerError("Failed to delete view column", id));

				}

				var success = _dboManager.Delete<TfSpaceView>(id);

				if (!success)
					return Result.Fail(new DboManagerError("Delete", id));

				scope.Complete();

				return Result.Ok();
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to delete space view.").CausedBy(ex));
		}
	}

	#region <--- validation --->

	internal class TfSpaceViewValidator
	: AbstractValidator<TfSpaceView>
	{
		public TfSpaceViewValidator(
			IDboManager dboManager,
			ITfSpaceManager spaceManager)
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

				RuleFor(spaceView => spaceView.SpaceDataId)
					.NotEmpty()
					.WithMessage("The space data id is required.");

				//TODO rumen more validation about space data - SpaceId and SpaceId in space view

			});

			RuleSet("create", () =>
			{
				RuleFor(spaceView => spaceView.Id)
						.Must((spaceView, id) => { return spaceManager.GetSpaceView(id).Value == null; })
						.WithMessage("There is already existing space view with specified identifier.");

				RuleFor(spaceView => spaceView.Name)
						.Must((spaceView, name) =>
						{
							if (string.IsNullOrEmpty(name))
								return true;

							var spaceViews = spaceManager.GetSpaceViewsList(spaceView.SpaceId).Value;
							return !spaceViews.Any(x => x.Name.ToLowerInvariant().Trim() == name.ToLowerInvariant().Trim());
						})
						.WithMessage("There is already existing space view with same name.");


			});

			RuleSet("update", () =>
			{
				RuleFor(spaceView => spaceView.Id)
						.Must((spaceView, id) =>
						{
							return spaceManager.GetSpaceView(id).Value != null;
						})
						.WithMessage("There is not existing space view with specified identifier.");


				//TODO rumen more validation about SpaceId and SpaceDataId changes


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
