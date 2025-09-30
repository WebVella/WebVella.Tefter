using System.Text.Json.Serialization.Metadata;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	public List<TfSpaceView> GetAllSpaceViews(string? search = null);

	public List<TfSpaceView> GetSpaceViewsList(
		Guid spaceId, string? search = null);

	public TfSpaceView GetSpaceView(
		Guid id);

	public TfSpaceView CreateSpaceView(
		TfCreateSpaceViewExtended spaceViewExt);

	public TfSpaceView CreateSpaceView(
		TfSpaceView spaceView);

	public TfSpaceView UpdateSpaceView(
		TfSpaceView spaceView);

	public TfSpaceView UpdateSpaceView(
		TfCreateSpaceViewExtended spaceViewExt);

	public void UpdateSpaceViewPresets(
		Guid spaceViewId,
		List<TfSpaceViewPreset> presets);

	public void DeleteSpaceView(
		Guid id);

	public TfSpaceView CopySpaceView(
		Guid id);

	public void MoveSpaceViewUp(
		Guid id);

	public void MoveSpaceViewDown(
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

	public TfSpaceView CreateSpaceView(
		TfCreateSpaceViewExtended spaceViewExt)
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

			if (spaceViewExt.SpaceDataId is null)
				valEx.AddValidationError(nameof(spaceViewExt.SpaceDataId), "required");
			else
			{
				dataset = GetDataset(spaceViewExt.SpaceDataId.Value);
				if (dataset is null)
					valEx.AddValidationError(nameof(spaceViewExt.SpaceDataId), "dataset is not found");
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

			//TODO RUMEN -> As Space ID will be removed from the view it is hardcoded here but needs to be removed
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
		TfCreateSpaceViewExtended spaceViewExt)
	{
		try
		{
			throw new NotImplementedException();
			//TfSpace space = null;
			//TfDataset spaceData = null;
			//TfSpaceView spaceView = null;
			//TfDataProvider dataprovider = null;
			//var createNewDataset = spaceViewExt.DatasetType == TfSpaceViewDatasetType.New;
			//#region << Validate>>

			//var valEx = new TfValidationException();

			////args
			//if (String.IsNullOrWhiteSpace(spaceViewExt.Name))
			//	valEx.AddValidationError(nameof(spaceViewExt.Name), "required");

			//if (spaceViewExt.SpaceId == Guid.Empty)
			//	valEx.AddValidationError(nameof(spaceViewExt.SpaceId), "required");

			//if (createNewDataset)
			//{
			//	if (String.IsNullOrWhiteSpace(spaceViewExt.NewSpaceDataName))
			//		valEx.AddValidationError(nameof(spaceViewExt.NewSpaceDataName), "required");

			//	if (spaceViewExt.DataProviderId is null)
			//		valEx.AddValidationError(nameof(spaceViewExt.DataProviderId), "required");
			//}
			//else
			//{
			//	if (spaceViewExt.SpaceDataId is null)
			//		valEx.AddValidationError(nameof(spaceViewExt.SpaceDataId), "required");
			//}

			////Space
			//space = GetSpace(spaceViewExt.SpaceId);
			//if (space is null)
			//	valEx.AddValidationError(nameof(spaceViewExt.SpaceId), "space is not found");

			////DataProvider
			//if (spaceViewExt.DataProviderId is not null)
			//{
			//	dataprovider = GetDataProvider(spaceViewExt.DataProviderId.Value);
			//	if (dataprovider is null)
			//		valEx.AddValidationError(nameof(spaceViewExt.DataProviderId), "data provider is not found");
			//}

			////SpaceData
			//if (spaceViewExt.SpaceDataId is not null)
			//{
			//	spaceData = GetDataset(spaceViewExt.SpaceDataId.Value);
			//	if (spaceData is null)
			//		valEx.AddValidationError(nameof(spaceViewExt.SpaceDataId), "dataset is not found");
			//}

			//valEx.ThrowIfContainsErrors();

			//#endregion

			//using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			//{
			//	#region << create space data if needed >>
			//	if (spaceData is null)
			//	{
			//		List<string> selectedColumns = new();
			//		//system columns are always selected so we should not add them in the space data
			//		if (spaceViewExt.AddProviderColumns && spaceViewExt.AddSharedColumns)
			//		{
			//			//all columns are requested from the provider, so send empty column list, which will apply newly added columns
			//			//to the provider dynamically
			//		}
			//		else if (spaceViewExt.AddProviderColumns)
			//		{
			//			selectedColumns.AddRange(dataprovider.Columns.Select(x => x.DbName).ToList());
			//		}
			//		else if (spaceViewExt.AddSharedColumns)
			//		{
			//			selectedColumns.AddRange(dataprovider.SharedColumns.Select(x => x.DbName).ToList());
			//		}

			//		var spaceDataObj = new TfCreateDataset()
			//		{
			//			Id = Guid.NewGuid(),
			//			Name = spaceViewExt.NewSpaceDataName,
			//			Filters = new(),//filters will not be added at this point
			//			Columns = selectedColumns,
			//			DataProviderId = dataprovider.Id,
			//		};

			//		spaceData = CreateDataset(spaceDataObj);
			//	}
			//	#endregion

			//	#region << update view>>
			//	{
			//		var spaceViewObj = new TfSpaceView()
			//		{
			//			Id = spaceViewExt.Id,
			//			Name = spaceViewExt.Name,
			//			Position = 1,//will be updated later
			//			DatasetId = spaceData.Id,
			//			SpaceId = space.Id,
			//			Type = spaceViewExt.Type,
			//			SettingsJson = spaceViewExt.SettingsJson,
			//			Presets = spaceViewExt.Presets,
			//		};
			//		spaceView = UpdateSpaceView(spaceViewObj);
			//	}
			//	#endregion

			//	scope.Complete();

			//	return GetSpaceView(spaceView.Id);
			//}
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

	public TfSpaceView CopySpaceView(
			Guid originalId)
	{
		TfSpaceView originalSV = GetSpaceView(originalId);
		if (originalSV is null)
			throw new Exception("Space view not found");

		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var spaceViewList = GetSpaceViewsList(originalSV.SpaceId);

				var copyName = getSpaceViewCopyName(originalSV.Name, spaceViewList);
				if (String.IsNullOrWhiteSpace(copyName))
					throw new Exception("Space data copy name could not be generated");

				TfSpaceView spaceView = new()
				{
					Id = Guid.NewGuid(),
					Name = copyName,
					SpaceId = originalSV.SpaceId,
					Position = 0,
					Presets = originalSV.Presets,
					SettingsJson = originalSV.SettingsJson,
					DatasetId = originalSV.DatasetId,
					SpaceDataName = originalSV.SpaceDataName,
					Type = originalSV.Type,
				};
				spaceView = CreateSpaceView(spaceView);

				var originalColumns = GetSpaceViewColumnsList(originalSV.Id);

				foreach (var orColumn in originalColumns)
				{
					_ = CreateSpaceViewColumn(new TfSpaceViewColumn
					{
						Id = Guid.NewGuid(),
						ComponentId = orColumn.ComponentId,
						ComponentOptionsJson = orColumn.ComponentOptionsJson,
						EditComponentOptionsJson = orColumn.EditComponentOptionsJson,
						DataMapping = orColumn.DataMapping,
						Icon = orColumn.Icon,
						OnlyIcon = orColumn.OnlyIcon,
						Position = orColumn.Position,
						QueryName = orColumn.QueryName,
						Settings = orColumn.Settings,
						SpaceViewId = spaceView.Id,
						Title = orColumn.Title,
						TypeId = orColumn.TypeId,
					});
				}

				scope.Complete();

				return spaceView;
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

	#region << Private >>
	private string? getSpaceViewCopyName(string originalName, List<TfSpaceView> spaceiewList)
	{
		var index = 1;
		var presentNamesHS = spaceiewList.Select(x => x.Name).ToHashSet();

		while (true)
		{
			var suggestion = $"{originalName} {index}";
			if (!presentNamesHS.Contains(suggestion))
				return suggestion;

			index++;

			if (index > 100000) break;
		}

		return null;
	}
	#endregion
}
