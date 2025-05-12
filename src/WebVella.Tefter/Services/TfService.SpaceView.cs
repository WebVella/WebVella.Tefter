using System.Text.Json.Serialization.Metadata;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	public List<TfSpaceView> GetAllSpaceViews();

	public List<TfSpaceView> GetSpaceViewsList(
		Guid spaceId);

	public TfSpaceView GetSpaceView(
		Guid id);

	public TfSpaceView CreateSpaceView(
		TfCreateSpaceViewExtended spaceViewExt,
		bool createNewDataSet = true);

	public TfSpaceView CreateSpaceView(
		TfSpaceView spaceView);

	public TfSpaceView UpdateSpaceView(
		TfSpaceView spaceView);

	public TfSpaceView UpdateSpaceView(
		TfCreateSpaceViewExtended spaceViewExt,
		bool createNewDataSet = true);

	public void UpdateSpaceViewPresets(
		Guid spaceViewId,
		List<TfSpaceViewPreset> presets);

	public void DeleteSpaceView(
		Guid id);

	public void MoveSpaceViewUp(
		Guid id);

	public void MoveSpaceViewDown(
		Guid id);
}

public partial class TfService : ITfService
{
	public List<TfSpaceView> GetAllSpaceViews()
	{
		try
		{
			var spaceViews = _dboManager.GetList<TfSpaceViewDbo>();
			return spaceViews.Select(x => ConvertDboToModel(x)).ToList();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public List<TfSpaceView> GetSpaceViewsList(
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

			return spaceViews.Select(x => ConvertDboToModel(x)).ToList();
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

	public TfSpaceView CreateSpaceView(
		TfCreateSpaceViewExtended spaceViewExt,
		bool createNewDataSet = true)
	{
		try
		{
			if (spaceViewExt != null && spaceViewExt.Id == Guid.Empty)
				spaceViewExt.Id = Guid.NewGuid();

			TfSpace space = null;
			TfSpaceData spaceData = null;
			TfSpaceView spaceView = null;
			TfDataProvider dataprovider = null;

			#region << Validate>>

			var valEx = new TfValidationException();

			//args
			if (String.IsNullOrWhiteSpace(spaceViewExt.Name))
				valEx.AddValidationError(nameof(spaceViewExt.Name), "required");

			if (spaceViewExt.SpaceId == Guid.Empty)
				valEx.AddValidationError(nameof(spaceViewExt.SpaceId), "required");

			if (createNewDataSet)
			{
				if (String.IsNullOrWhiteSpace(spaceViewExt.NewSpaceDataName))
					valEx.AddValidationError(nameof(spaceViewExt.NewSpaceDataName), "required");

				if (spaceViewExt.DataProviderId is null)
					valEx.AddValidationError(nameof(spaceViewExt.DataProviderId), "required");
			}
			else
			{
				if (spaceViewExt.SpaceDataId is null)
					valEx.AddValidationError(nameof(spaceViewExt.SpaceDataId), "required");
			}

			//Space
			space = GetSpace(spaceViewExt.SpaceId);
			if (space is null)
				valEx.AddValidationError(nameof(spaceViewExt.SpaceId), "space is not found");


			//SpaceData
			if (spaceViewExt.SpaceDataId is not null)
			{
				spaceData = GetSpaceData(spaceViewExt.SpaceDataId.Value);
				if (spaceData is null)
					valEx.AddValidationError(nameof(spaceViewExt.SpaceDataId), "dataset is not found");
			}

			//DataProvider
			Guid? dataProviderId = null;
			if (spaceViewExt.DataProviderId is not null)
			{
				dataProviderId = spaceViewExt.DataProviderId.Value;
			}
			else if (spaceData is not null)
			{
				dataProviderId = spaceData.DataProviderId;
			}

			if (dataProviderId is not null)
			{
				dataprovider = GetDataProvider(dataProviderId.Value);
				if (dataprovider is null)
					valEx.AddValidationError(nameof(dataProviderId), "data provider is not found");
			}

			valEx.ThrowIfContainsErrors();

			#endregion

			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
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
					else if (spaceViewExt.AddProviderColumns)
					{
						selectedColumns.AddRange(dataprovider.Columns.Select(x => x.DbName).ToList());
					}
					else if (spaceViewExt.AddSharedColumns)
					{
						selectedColumns.AddRange(dataprovider.SharedColumns.Select(x => x.DbName).ToList());
					}

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

					spaceData = CreateSpaceData(spaceDataObj);
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
						Presets = spaceViewExt.Presets,
						SettingsJson = spaceViewExt.SettingsJson,
					};

					spaceView = CreateSpaceView(spaceViewObj);
				}
				#endregion

				#region << create view columns>>
				{
					var availableTypes = GetAvailableSpaceViewColumnTypes();
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
									ComponentOptionsJson = "{}",
									DataMapping = new(),
									TypeId = Guid.Empty,
									ComponentId = Guid.Empty,
								};

								if (columnType is not null)
								{
									tfColumn.TypeId = columnType.AddonId;
									tfColumn.ComponentId = columnType.DefaultComponentId ?? new Guid(TfTextDisplayColumnComponent.ID);
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
									ComponentOptionsJson = "{}",
									DataMapping = new(),
									TypeId = Guid.Empty,
									ComponentId = Guid.Empty,
								};

								if (columnType is not null)
								{
									tfColumn.TypeId = columnType.AddonId;
									tfColumn.ComponentId = columnType.DefaultComponentId ?? new Guid(TfTextDisplayColumnComponent.ID);
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
									ComponentOptionsJson = "{}",
									DataMapping = new(),
									TypeId = Guid.Empty,
									ComponentId = Guid.Empty,
								};

								if (columnType is not null)
								{
									tfColumn.TypeId = columnType.AddonId;
									tfColumn.ComponentId = columnType.DefaultComponentId ?? new Guid(TfTextDisplayColumnComponent.ID);
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
							var columnsList = new List<string>();
							if (spaceData.Columns.Count > 0)
							{
								columnsList = spaceData.Columns;
							}
							else
							{
								columnsList = dataprovider.Columns.Select(x => x.DbName).ToList();
							}
							foreach (var dbName in columnsList)
							{
								TfDatabaseColumnType? dbType = dataprovider.Columns.FirstOrDefault(x => x.DbName == dbName)?.DbType;

								if (dbType is null)
									dbType = dataprovider.SharedColumns.FirstOrDefault(x => x.DbName == dbName)?.DbType;

								if (dbType is null)
									dbType = dataprovider.SystemColumns.FirstOrDefault(x => x.DbName == dbName)?.DbType;

								if (dbType is null) continue;

								var columnType = ModelHelpers.GetColumnTypeForDbType(dbType.Value, availableTypes);
								var tfColumn = new TfSpaceViewColumn
								{
									Id = Guid.NewGuid(),
									SpaceViewId = spaceView.Id,
									Position = position,
									Title = dbName,
									QueryName = dbName,
									ComponentOptionsJson = "{}",
									DataMapping = new(),
									TypeId = Guid.Empty,
									ComponentId = Guid.Empty
								};

								if (columnType is not null)
								{
									tfColumn.TypeId = columnType.AddonId;
									tfColumn.ComponentId = columnType.DefaultComponentId ?? new Guid(TfTextDisplayColumnComponent.ID);
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
						var createdColumn = CreateSpaceViewColumn(tfColumn);
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

	public void UpdateSpaceViewPresets(
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

	public TfSpaceView UpdateSpaceView(
		TfCreateSpaceViewExtended spaceViewExt,
		bool createNewDataSet = true)
	{
		try
		{
			TfSpace space = null;
			TfSpaceData spaceData = null;
			TfSpaceView spaceView = null;
			TfDataProvider dataprovider = null;

			#region << Validate>>

			var valEx = new TfValidationException();

			//args
			if (String.IsNullOrWhiteSpace(spaceViewExt.Name))
				valEx.AddValidationError(nameof(spaceViewExt.Name), "required");

			if (spaceViewExt.SpaceId == Guid.Empty)
				valEx.AddValidationError(nameof(spaceViewExt.SpaceId), "required");

			if (createNewDataSet)
			{
				if (String.IsNullOrWhiteSpace(spaceViewExt.NewSpaceDataName))
					valEx.AddValidationError(nameof(spaceViewExt.NewSpaceDataName), "required");

				if (spaceViewExt.DataProviderId is null)
					valEx.AddValidationError(nameof(spaceViewExt.DataProviderId), "required");
			}
			else
			{
				if (spaceViewExt.SpaceDataId is null)
					valEx.AddValidationError(nameof(spaceViewExt.SpaceDataId), "required");
			}

			//Space
			space = GetSpace(spaceViewExt.SpaceId);
			if (space is null)
				valEx.AddValidationError(nameof(spaceViewExt.SpaceId), "space is not found");

			//DataProvider
			if (spaceViewExt.DataProviderId is not null)
			{
				dataprovider = GetDataProvider(spaceViewExt.DataProviderId.Value);
				if (dataprovider is null)
					valEx.AddValidationError(nameof(spaceViewExt.DataProviderId), "data provider is not found");
			}

			//SpaceData
			if (spaceViewExt.SpaceDataId is not null)
			{
				spaceData = GetSpaceData(spaceViewExt.SpaceDataId.Value);
				if (spaceData is null)
					valEx.AddValidationError(nameof(spaceViewExt.SpaceDataId), "dataset is not found");
			}

			valEx.ThrowIfContainsErrors();

			#endregion

			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
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
					else if (spaceViewExt.AddProviderColumns)
					{
						selectedColumns.AddRange(dataprovider.Columns.Select(x => x.DbName).ToList());
					}
					else if (spaceViewExt.AddSharedColumns)
					{
						selectedColumns.AddRange(dataprovider.SharedColumns.Select(x => x.DbName).ToList());
					}

					var spaceDataObj = new TfSpaceData()
					{
						Id = Guid.NewGuid(),
						Name = spaceViewExt.NewSpaceDataName,
						Filters = new(),//filters will not be added at this point
						Columns = selectedColumns,
						DataProviderId = dataprovider.Id,
						SpaceId = space.Id,
						Position = 1 //position is updated in the creation
					};

					spaceData = CreateSpaceData(spaceDataObj);
				}
				#endregion

				#region << update view>>
				{
					var spaceViewObj = new TfSpaceView()
					{
						Id = spaceViewExt.Id,
						Name = spaceViewExt.Name,
						Position = 1,//will be updated later
						SpaceDataId = spaceData.Id,
						SpaceId = space.Id,
						Type = spaceViewExt.Type,
						SettingsJson = spaceViewExt.SettingsJson,
						Presets = spaceViewExt.Presets,
					};
					spaceView = UpdateSpaceView(spaceViewObj);
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

	public void MoveSpaceViewUp(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var spaceView = GetSpaceView(id);

				if (spaceView == null)
				{
					throw new TfValidationException(nameof(id),
						"Found no space view for specified identifier.");
				}

				if (spaceView.Position == 1)
					return;

				var spaceViews = GetSpaceViewsList(spaceView.SpaceId);

				var prevSpace = spaceViews.SingleOrDefault(x => x.Position == (spaceView.Position - 1));
				spaceView.Position = (short)(spaceView.Position - 1);

				if (prevSpace != null)
					prevSpace.Position = (short)(prevSpace.Position + 1);

				var success = _dboManager.Update<TfSpaceViewDbo>(ConvertModelToDbo(spaceView));
				if (!success)
					throw new TfDboServiceException("Update<TfSpaceViewDbo> failed");

				if (prevSpace != null)
				{
					success = _dboManager.Update<TfSpaceViewDbo>(ConvertModelToDbo(prevSpace));
					if (!success)
						throw new TfDboServiceException("Update<TfSpaceViewDbo> failed");
				}

				scope.Complete();
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void MoveSpaceViewDown(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var spaceView = GetSpaceView(id);

				if (spaceView == null)
				{
					throw new TfValidationException(nameof(id),
						"Found no space view for specified identifier.");
				}

				var spaceViews = GetSpaceViewsList(spaceView.SpaceId);

				if (spaceView.Position == spaceViews.Count)
					return;

				var nextSpaceView = spaceViews.SingleOrDefault(x => x.Position == (spaceView.Position + 1));
				spaceView.Position = (short)(spaceView.Position + 1);

				if (nextSpaceView != null)
					nextSpaceView.Position = (short)(nextSpaceView.Position - 1);

				var success = _dboManager.Update<TfSpaceViewDbo>(ConvertModelToDbo(spaceView));
				if (!success)
					throw new TfDboServiceException("Update<TfSpaceViewDbo> failed");

				if (nextSpaceView != null)
				{
					success = _dboManager.Update<TfSpaceViewDbo>(ConvertModelToDbo(nextSpaceView));
					if (!success)
						throw new TfDboServiceException("Update<TfSpaceViewDbo> failed");
				}

				scope.Complete();
			}
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
			SpaceDataId = dbo.SpaceDataId,
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
			SpaceDataId = model.SpaceDataId,
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

				RuleFor(spaceView => spaceView.SpaceDataId)
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
