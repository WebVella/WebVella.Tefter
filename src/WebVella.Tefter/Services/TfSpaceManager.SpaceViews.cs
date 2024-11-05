using System.Text.Json.Serialization.Metadata;

namespace WebVella.Tefter;

public partial interface ITfSpaceManager
{

	public Result<List<TfSpaceView>> GetAllSpaceViews();
	public Result<List<TfSpaceView>> GetSpaceViewsList(
		Guid spaceId);

	public Result<TfSpaceView> GetSpaceView(
		Guid id);

	public Result<TfSpaceView> CreateSpaceView(
		TfCreateSpaceViewExtended spaceViewExt, bool createNewDataSet = true);

	public Result<TfSpaceView> CreateSpaceView(
		TfSpaceView spaceView);

	public Result<TfSpaceView> UpdateSpaceView(
		TfSpaceView spaceView);

	public Result<TfSpaceView> UpdateSpaceView(
		TfCreateSpaceViewExtended spaceViewExt, bool createNewDataSet = true);

	public Result UpdateSpaceViewPresets(
		Guid spaceViewId,
		List<TfSpaceViewPreset> presets);

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
			var spaceViews = _dboManager.GetList<TfSpaceViewDbo>();

			return Result.Ok(spaceViews.Select(x => Convert(x)).ToList());
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
			var orderSettings = new TfOrderSettings(
				nameof(TfSpace.Position),
				OrderDirection.ASC);

			var spaceViews = _dboManager.GetList<TfSpaceViewDbo>(
				spaceId,
				nameof(TfSpaceView.SpaceId),
				order: orderSettings);

			return Result.Ok(spaceViews.Select(x => Convert(x)).ToList());
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
			var spaceView = _dboManager.Get<TfSpaceViewDbo>(id);
			return Result.Ok(Convert(spaceView));
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get space view by id").CausedBy(ex));
		}
	}

	public Result<TfSpaceView> CreateSpaceView(
		TfCreateSpaceViewExtended spaceViewExt, bool createNewDataSet = true)
	{
		//TODO RUMEN: refactor metod to standard
		try
		{
			if (spaceViewExt != null && spaceViewExt.Id == Guid.Empty)
				spaceViewExt.Id = Guid.NewGuid();

			TfSpace space = null;
			TfSpaceData spaceData = null;
			TfSpaceView spaceView = null;
			TfDataProvider dataprovider = null;

			#region << Validate>>
			var validationErrors = new List<ValidationError>();
			//args
			if (String.IsNullOrWhiteSpace(spaceViewExt.Name)) validationErrors.Add(new ValidationError(nameof(spaceViewExt.Name), "required"));
			if (spaceViewExt.SpaceId == Guid.Empty) validationErrors.Add(new ValidationError(nameof(spaceViewExt.SpaceId), "required"));
			if (createNewDataSet)
			{
				if (String.IsNullOrWhiteSpace(spaceViewExt.NewSpaceDataName)) validationErrors.Add(new ValidationError(nameof(spaceViewExt.NewSpaceDataName), "required"));
				if (spaceViewExt.DataProviderId is null) validationErrors.Add(new ValidationError(nameof(spaceViewExt.DataProviderId), "required"));
			}
			else
			{
				if (spaceViewExt.SpaceDataId is null) validationErrors.Add(new ValidationError(nameof(spaceViewExt.SpaceDataId), "required"));
			}

			//Space
			var spaceResult = GetSpace(spaceViewExt.SpaceId);
			if (spaceResult.IsFailed) return Result.Fail(new Error("GetSpace failed").CausedBy(spaceResult.Errors));
			if (spaceResult.Value is null) validationErrors.Add(new ValidationError(nameof(spaceViewExt.SpaceId), "space is not found"));
			space = spaceResult.Value;

			//SpaceData
			if (spaceViewExt.SpaceDataId is not null)
			{
				var spaceDataResult = GetSpaceData(spaceViewExt.SpaceDataId.Value);
				if (spaceDataResult.IsFailed) return Result.Fail(new Error("GetSpaceData failed").CausedBy(spaceDataResult.Errors));
				if (spaceDataResult.Value is null) validationErrors.Add(new ValidationError(nameof(spaceViewExt.SpaceDataId), "dataset is not found"));
				spaceData = spaceDataResult.Value;
			}

			//DataProvider
			Guid? dataProviderId = null;
			if (spaceViewExt.DataProviderId is not null) dataProviderId = spaceViewExt.DataProviderId.Value;
			else if (spaceData is not null) dataProviderId = spaceData.DataProviderId;
			if (dataProviderId is not null)
			{
				var providerResult = _providerManager.GetProvider(dataProviderId.Value);
				if (providerResult.IsFailed) return Result.Fail(new Error("GetProvider failed").CausedBy(providerResult.Errors));
				if (providerResult.Value is null) validationErrors.Add(new ValidationError(nameof(dataProviderId), "data provider is not found"));
				dataprovider = providerResult.Value;
			}

			if (validationErrors.Count > 0)
				return Result.Fail(validationErrors);

			#endregion

			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				#region << create space data if needed >>
				if (spaceData is null)
				{
					List<string> selectedColumns = new();
					//system columns are always selected so we should not add them in the space data
					if (spaceViewExt.AddProviderColumns && spaceViewExt.AddSharedColumns)
					{
						//all columns are requested from the provider, so send empty column list, which will apply newly added columns
						//to the provider dynamically
					}
					else if (spaceViewExt.AddProviderColumns) selectedColumns.AddRange(dataprovider.Columns.Select(x => x.DbName).ToList());
					else if (spaceViewExt.AddSharedColumns) selectedColumns.AddRange(dataprovider.SharedColumns.Select(x => x.DbName).ToList());

					var spaceDataObj = new TfSpaceData()
					{
						Id = Guid.NewGuid(),
						Name = spaceViewExt.NewSpaceDataName,
						Filters = new(),//filters will not be added at this point
						Columns = selectedColumns,
						DataProviderId = dataprovider.Id,
						SpaceId = space.Id,
						Position = 1 //position is overrided in the creation
					};

					var tfResult = CreateSpaceData(spaceDataObj);
					if (tfResult.IsFailed) return Result.Fail(new Error("CreateSpaceData failed").CausedBy(tfResult.Errors));
					if (tfResult.Value is null) return Result.Fail("CreateSpaceData failed to return value");
					spaceData = tfResult.Value;
				}
				#endregion

				#region << create view>>
				{
					var spaceViewObj = new TfSpaceView()
					{
						Id = Guid.NewGuid(),
						Name = spaceViewExt.Name,
						Position = 1,//will be overrided later
						SpaceDataId = spaceData.Id,
						SpaceId = space.Id,
						Type = spaceViewExt.Type,
						Groups = spaceViewExt.Groups,
						Presets = spaceViewExt.Presets,
						SettingsJson = spaceViewExt.SettingsJson,
					};
					var tfResult = CreateSpaceView(spaceViewObj);
					if (tfResult.IsFailed) return Result.Fail(new Error("CreateSpaceView failed").CausedBy(tfResult.Errors));
					if (tfResult.Value is null) return Result.Fail("CreateSpaceView failed to return value");
					spaceView = tfResult.Value;
				}
				#endregion

				#region << create view columns>>
				{
					var availableTypes = GetAvailableSpaceViewColumnTypes().Value;
					var columnsToCreate = new List<TfSpaceViewColumn>();
					short position = 1;
					if (createNewDataSet)
					{
						if (spaceViewExt.AddProviderColumns)
						{
							foreach (var column in dataprovider.Columns)
							{
								var columnType = ModelHelpers.GetColumnTypeForDbType(column.DbType, availableTypes);
								var tfColumn = new TfSpaceViewColumn
								{
									Id = Guid.NewGuid(),
									SpaceViewId = spaceView.Id,
									Position = position,
									Title = column.DbName,
									QueryName = column.DbName,
									CustomOptionsJson = "{}",
									DataMapping = new(),
									ColumnType = null,
									ComponentType = null,
									FullComponentTypeName = null,
									FullTypeName = null,
								};

								if (columnType is not null)
								{
									tfColumn.ColumnType = columnType;
									tfColumn.ComponentType = columnType.DefaultComponentType;
									tfColumn.FullComponentTypeName = columnType.DefaultComponentType.FullName;
									tfColumn.FullTypeName = columnType.Name;
									foreach (var mapper in columnType.DataMapping)
									{
										tfColumn.DataMapping[mapper.Alias] = column.DbName;
									}
								}
								columnsToCreate.Add(tfColumn);
								position++;
							}
						}
						if (spaceViewExt.AddSharedColumns)
						{
							foreach (var column in dataprovider.SharedColumns)
							{
								var columnType = ModelHelpers.GetColumnTypeForDbType(column.DbType, availableTypes);
								var tfColumn = new TfSpaceViewColumn
								{
									Id = Guid.NewGuid(),
									SpaceViewId = spaceView.Id,
									Position = position,
									Title = column.DbName,
									QueryName = column.DbName,
									CustomOptionsJson = "{}",
									DataMapping = new(),
									ColumnType = null,
									ComponentType = null,
									FullComponentTypeName = null,
									FullTypeName = null
								};

								if (columnType is not null)
								{
									tfColumn.ColumnType = columnType;
									tfColumn.ComponentType = columnType.DefaultComponentType;
									tfColumn.FullComponentTypeName = columnType.DefaultComponentType.FullName;
									tfColumn.FullTypeName = columnType.Name;
									foreach (var mapper in columnType.DataMapping)
									{
										tfColumn.DataMapping[mapper.Alias] = column.DbName;
									}
								}
								columnsToCreate.Add(tfColumn);
								position++;
							}
						}
						if (spaceViewExt.AddSystemColumns)
						{
							foreach (var column in dataprovider.SystemColumns)
							{
								var columnType = ModelHelpers.GetColumnTypeForDbType(column.DbType, availableTypes);
								var tfColumn = new TfSpaceViewColumn
								{
									Id = Guid.NewGuid(),
									SpaceViewId = spaceView.Id,
									Position = position,
									Title = column.DbName,
									QueryName = column.DbName,
									CustomOptionsJson = "{}",
									DataMapping = new(),
									ColumnType = null,
									ComponentType = null,
									FullComponentTypeName = null,
									FullTypeName = null,
								};

								if (columnType is not null)
								{
									tfColumn.ColumnType = columnType;
									tfColumn.ComponentType = columnType.DefaultComponentType;
									tfColumn.FullComponentTypeName = columnType.DefaultComponentType.FullName;
									tfColumn.FullTypeName = columnType.Name;
									foreach (var mapper in columnType.DataMapping)
									{
										tfColumn.DataMapping[mapper.Alias] = column.DbName;
									}
								}
								columnsToCreate.Add(tfColumn);
								position++;
							}
						}
					}
					else
					{
						if (spaceViewExt.AddDataSetColumns)
						{
							foreach (var dbName in spaceData.Columns)
							{
								TfDatabaseColumnType? dbType = dataprovider.Columns.FirstOrDefault(x => x.DbName == dbName)?.DbType;
								if (dbType is null) dbType = dataprovider.SharedColumns.FirstOrDefault(x => x.DbName == dbName)?.DbType;
								if (dbType is null) dbType = dataprovider.SystemColumns.FirstOrDefault(x => x.DbName == dbName)?.DbType;
								if (dbType is null) continue;

								var columnType = ModelHelpers.GetColumnTypeForDbType(dbType.Value, availableTypes);
								var tfColumn = new TfSpaceViewColumn
								{
									Id = Guid.NewGuid(),
									SpaceViewId = spaceView.Id,
									Position = position,
									Title = dbName,
									QueryName = dbName,
									CustomOptionsJson = "{}",
									DataMapping = new(),
									ColumnType = null,
									ComponentType = null,
									FullComponentTypeName = null,
									FullTypeName = null,
								};

								if (columnType is not null)
								{
									tfColumn.ColumnType = columnType;
									tfColumn.ComponentType = columnType.DefaultComponentType;
									tfColumn.FullComponentTypeName = columnType.DefaultComponentType.FullName;
									tfColumn.FullTypeName = columnType.Name;
									foreach (var mapper in columnType.DataMapping)
									{
										tfColumn.DataMapping[mapper.Alias] = dbName;
									}
								}
								columnsToCreate.Add(tfColumn);
								position++;
							}
						}
					}
					foreach (var tfColumn in columnsToCreate)
					{
						var tfResult = CreateSpaceViewColumn(tfColumn);
						if (tfResult.IsFailed) return Result.Fail(new Error("CreateSpaceViewColumn failed").CausedBy(tfResult.Errors));
						if (tfResult.Value is null) return Result.Fail("CreateSpaceViewColumn failed to return value");
					}
				}
				#endregion

				scope.Complete();

				return GetSpaceView(spaceView.Id);
			}
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

				var success = _dboManager.Insert<TfSpaceViewDbo>(Convert(spaceView));

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

	public Result UpdateSpaceViewPresets(
		Guid spaceViewId,
		List<TfSpaceViewPreset> presets)
	{
		try
		{
			var existingSpaceView = _dboManager.Get<TfSpaceViewDbo>(spaceViewId);
			if (existingSpaceView == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The space view is null.") }).ToResult();


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
				return Result.Fail(new DboManagerError("Update", existingSpaceView));

			return Result.Ok();
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to update space").CausedBy(ex));
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

			var existingSpaceView = _dboManager.Get<TfSpaceViewDbo>(spaceView.Id);

			//position is not updated
			spaceView.Position = existingSpaceView.Position;

			var success = _dboManager.Update<TfSpaceViewDbo>(Convert(spaceView));

			if (!success)
				return Result.Fail(new DboManagerError("Update", spaceView));

			return GetSpaceView(spaceView.Id);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to update space").CausedBy(ex));
		}

	}

	public Result<TfSpaceView> UpdateSpaceView(
		TfCreateSpaceViewExtended spaceViewExt, bool createNewDataSet = true)
	{
		try
		{
			TfSpace space = null;
			TfSpaceData spaceData = null;
			TfSpaceView spaceView = null;
			TfDataProvider dataprovider = null;

			#region << Validate>>
			var validationErrors = new List<ValidationError>();
			//args
			if (String.IsNullOrWhiteSpace(spaceViewExt.Name)) validationErrors.Add(new ValidationError(nameof(spaceViewExt.Name), "required"));
			if (spaceViewExt.SpaceId == Guid.Empty) validationErrors.Add(new ValidationError(nameof(spaceViewExt.SpaceId), "required"));
			if (createNewDataSet)
			{
				if (String.IsNullOrWhiteSpace(spaceViewExt.NewSpaceDataName)) validationErrors.Add(new ValidationError(nameof(spaceViewExt.NewSpaceDataName), "required"));
				if (spaceViewExt.DataProviderId is null) validationErrors.Add(new ValidationError(nameof(spaceViewExt.DataProviderId), "required"));
			}
			else
			{
				if (spaceViewExt.SpaceDataId is null) validationErrors.Add(new ValidationError(nameof(spaceViewExt.SpaceDataId), "required"));
			}

			//Space
			var spaceResult = GetSpace(spaceViewExt.SpaceId);
			if (spaceResult.IsFailed) return Result.Fail(new Error("GetSpace failed").CausedBy(spaceResult.Errors));
			if (spaceResult.Value is null) validationErrors.Add(new ValidationError(nameof(spaceViewExt.SpaceId), "space is not found"));
			space = spaceResult.Value;

			//DataProvider
			if (spaceViewExt.DataProviderId is not null)
			{
				var providerResult = _providerManager.GetProvider(spaceViewExt.DataProviderId.Value);
				if (providerResult.IsFailed) return Result.Fail(new Error("GetProvider failed").CausedBy(providerResult.Errors));
				if (providerResult.Value is null) validationErrors.Add(new ValidationError(nameof(spaceViewExt.DataProviderId), "data provider is not found"));
				dataprovider = providerResult.Value;
			}

			//SpaceData
			if (spaceViewExt.SpaceDataId is not null)
			{
				var spaceDataResult = GetSpaceData(spaceViewExt.SpaceDataId.Value);
				if (spaceDataResult.IsFailed) return Result.Fail(new Error("GetSpaceData failed").CausedBy(spaceDataResult.Errors));
				if (spaceDataResult.Value is null) validationErrors.Add(new ValidationError(nameof(spaceViewExt.SpaceDataId), "dataset is not found"));
				spaceData = spaceDataResult.Value;
			}

			if (validationErrors.Count > 0)
				return Result.Fail(validationErrors);

			#endregion

			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				#region << create space data if needed >>
				if (spaceData is null)
				{
					List<string> selectedColumns = new();
					//system columns are always selected so we should not add them in the space data
					if (spaceViewExt.AddProviderColumns && spaceViewExt.AddSharedColumns)
					{
						//all columns are requested from the provider, so send empty column list, which will apply newly added columns
						//to the provider dynamically
					}
					else if (spaceViewExt.AddProviderColumns) selectedColumns.AddRange(dataprovider.Columns.Select(x => x.DbName).ToList());
					else if (spaceViewExt.AddSharedColumns) selectedColumns.AddRange(dataprovider.SharedColumns.Select(x => x.DbName).ToList());

					var spaceDataObj = new TfSpaceData()
					{
						Id = Guid.NewGuid(),
						Name = spaceViewExt.NewSpaceDataName,
						Filters = new(),//filters will not be added at this point
						Columns = selectedColumns,
						DataProviderId = dataprovider.Id,
						SpaceId = space.Id,
						Position = 1 //position is overrided in the creation
					};

					var tfResult = CreateSpaceData(spaceDataObj);
					if (tfResult.IsFailed) return Result.Fail(new Error("CreateSpaceData failed").CausedBy(tfResult.Errors));
					if (tfResult.Value is null) return Result.Fail("CreateSpaceData failed to return value");
					spaceData = tfResult.Value;
				}
				#endregion

				#region << update view>>
				{
					var spaceViewObj = new TfSpaceView()
					{
						Id = spaceViewExt.Id,
						Name = spaceViewExt.Name,
						Position = 1,//will be overrided later
						SpaceDataId = spaceData.Id,
						SpaceId = space.Id,
						Type = spaceViewExt.Type,
						SettingsJson = spaceViewExt.SettingsJson,
						Groups = spaceViewExt.Groups,
						Presets = spaceViewExt.Presets,
					};
					var tfResult = UpdateSpaceView(spaceViewObj);
					if (tfResult.IsFailed) return Result.Fail(new Error("UpdateSpaceView failed").CausedBy(tfResult.Errors));
					if (tfResult.Value is null) return Result.Fail("UpdateSpaceView failed to return value");
					spaceView = tfResult.Value;
				}
				#endregion

				scope.Complete();

				return GetSpaceView(spaceView.Id);
			}
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

				var success = _dboManager.Update<TfSpaceViewDbo>(Convert(spaceView));

				if (!success)
					return Result.Fail(new DboManagerError("Update", spaceView));

				if (prevSpace != null)
				{
					success = _dboManager.Update<TfSpaceViewDbo>(Convert(prevSpace));

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

				var success = _dboManager.Update<TfSpaceViewDbo>(Convert(spaceView));

				if (!success)
					return Result.Fail(new DboManagerError("Update", spaceView));

				if (nextSpaceView != null)
				{
					success = _dboManager.Update<TfSpaceViewDbo>(Convert(nextSpaceView));

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

				bool success = false;

				var bookmarks = GetSpaceViewBookmarksList(id).Value;
				foreach (var bookmark in bookmarks)
				{
					var result = DeleteBookmark(bookmark.Id);

					if (!result.IsSuccess)
						return Result.Fail(new DboManagerError("Failed to delete space view bookmark, " +
							"before deleting space view", bookmark));
				}

				var spacesAfter = GetSpaceViewsList(spaceView.SpaceId)
					.Value
					.Where(x => x.Position > spaceView.Position).ToList();

				//update positions for spaces after the one being deleted
				foreach (var spaceAfter in spacesAfter)
				{
					spaceAfter.Position--;
					var successUpdatePosition = _dboManager.Update<TfSpaceViewDbo>(Convert(spaceAfter));

					if (!successUpdatePosition)
						return Result.Fail(new DboManagerError("Failed to update space view position" +
							" during delete space process", spaceAfter));
				}

				var spaceViewColumns = GetSpaceViewColumnsList(spaceView.Id).Value;

				foreach (var column in spaceViewColumns)
				{
					var columnDeleteResult = DeleteSpaceViewColumn(column.Id);

					if (!columnDeleteResult.IsSuccess)
						return Result.Fail(new DboManagerError("Failed to delete view column", id));

				}

				success = _dboManager.Delete<TfSpaceViewDbo>(id);

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

	private TfSpaceView Convert(
		TfSpaceViewDbo dbo)
	{
		if (dbo == null)
			return null;

		List<string> groups = new List<string>();
		if (!string.IsNullOrWhiteSpace(dbo.GroupsJson))
			groups = JsonSerializer.Deserialize<List<string>>(dbo.GroupsJson);

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
			SpaceDataId = dbo.SpaceDataId,
			SpaceId = dbo.SpaceId,
			Type = dbo.Type,
			Groups = groups,
			Presets = presets
		};

	}

	private TfSpaceViewDbo Convert(
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
			SpaceDataId = model.SpaceDataId,
			SpaceId = model.SpaceId,
			Type = model.Type,
			GroupsJson = JsonSerializer.Serialize(model.Groups ?? new List<string>()),
			PresetsJson = JsonSerializer.Serialize(model.Presets ?? new List<TfSpaceViewPreset>())
		};
	}

	#region <--- validation --->

	internal class TfSpaceViewValidator
	: AbstractValidator<TfSpaceView>
	{
		public TfSpaceViewValidator(
			ITfDboManager dboManager,
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

				//RuleFor(spaceView => spaceView.Name)
				//		.Must((spaceView, name) =>
				//		{
				//			if (string.IsNullOrEmpty(name))
				//				return true;

				//			var spaceViews = spaceManager.GetSpaceViewsList(spaceView.SpaceId).Value;
				//			return !spaceViews.Any(x => x.Name.ToLowerInvariant().Trim() == name.ToLowerInvariant().Trim());
				//		})
				//		.WithMessage("There is already existing space view with same name.");


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
