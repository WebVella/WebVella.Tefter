using System.Text.Json.Serialization.Metadata;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	public List<TfSpaceView> GetAllSpaceViews(string? search = null);

	//public List<TfSpaceView> GetSpaceViewsList(
	//	Guid spaceId, string? search = null);

	public TfSpaceView? GetSpaceView(
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

	public void DeleteSpaceView(
		Guid id);
	
	public Task AddSpaceViewPreset(
		Guid spaceViewId,
		TfSpaceViewPreset preset);

	public Task UpdateSpaceViewPreset(
		Guid spaceViewId,
		TfSpaceViewPreset preset);

	public Task CopySpaceViewPreset(
		Guid spaceViewId,
		Guid presetId);	
	
	public Task MoveSpaceViewPreset(
		Guid spaceViewId,
		Guid presetId,
		bool isUp);		
	
	public Task RemoveSpaceViewPreset(
		Guid spaceViewId,
		Guid presetId);		
}

public partial class TfService
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

	public TfSpaceView? GetSpaceView(
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
						var datasetProvider = providers.Single(x => x.Id == dataset.DataProviderId);
						datasetProvider.Columns.ToList().ForEach(x => columnsList.Add(x.DbName!));
					}


					foreach (var columnName in columnsList)
					{
						TfDatabaseColumnType? dbType = null;
						if (!columnName.Contains("."))
						{
							dbType = columNameDbTypeDict.ContainsKey(columnName)
								? columNameDbTypeDict[columnName]
								: null;
						}
						else
						{
							var columnArray = columnName.Split('.');
							dbType = columNameDbTypeDict.ContainsKey(columnArray[1])
								? columNameDbTypeDict[columnArray[1]]
								: null;
						}

						if (dbType is null) continue;

						var columnType = ModelHelpers.GetColumnTypeForDbType(dbType.Value, availableTypes);
						var tfColumn = new TfSpaceViewColumn
						{
							Id = Guid.NewGuid(),
							SpaceViewId = spaceView.Id,
							Position = position,
							Title = NavigatorExt.ProcessForTitle(columnName),
							QueryName = NavigatorExt.GenerateQueryName(),
							TypeOptionsJson = "{}",
							DataMapping = new(),
							TypeId = Guid.Empty
						};

						if (columnType is not null)
						{
							tfColumn.TypeId = columnType.AddonId;
							//TODO initialize type options with default values for the column type
							//tfColumn.ComponentId = columnType.DefaultDisplayComponentId ?? new Guid(TucTextDisplayColumnComponent.ID);
							//tfColumn.EditComponentId = columnType.DefaultEditComponentId ?? new Guid(TucTextEditColumnComponent.ID);
							foreach (var mapper in columnType.DataMappingDefinitions)
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

			new TfSpaceViewValidator(this)
				.ValidateCreate(spaceView)
				.ToValidationException()
				.ThrowIfContainsErrors();

			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
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
			await PublishEventWithScopeAsync(new TfSpaceViewUpdatedEvent(spaceView));
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfSpaceView UpdateSpaceView(
		TfSpaceView spaceView)
	{
		new TfSpaceViewValidator(this)
			.ValidateUpdate(spaceView)
			.ToValidationException()
			.ThrowIfContainsErrors();

		var success = _dboManager.Update<TfSpaceViewDbo>(ConvertModelToDbo(spaceView));

		if (!success)
			throw new TfDboServiceException("Update<TfSpaceViewDbo> failed.");

		return GetSpaceView(spaceView.Id);
	}

	[Obsolete("Views can not be deleted by themselves any more, as they are connected to the page")]
	public void DeleteSpaceView(
		Guid id)
	{
		using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
		{
			var spaceView = GetSpaceView(id);

			new TfSpaceViewValidator(this)
				.ValidateDelete(spaceView)
				.ToValidationException()
				.ThrowIfContainsErrors();

			bool success = false;

			var bookmarks = GetBookmarksListForSpace(id);
			foreach (var bookmark in bookmarks)
			{
				DeleteBookmark(bookmark.Id);
			}

			var spaceViewColumns = GetSpaceViewColumnsList(spaceView.Id);
			foreach (var column in spaceViewColumns)
			{
				//delete method was changed to async in order to support event publishing
				DeleteSpaceViewColumn(column.Id).GetAwaiter().GetResult();
			}

			success = _dboManager.Delete<TfSpaceViewDbo>(id);
			if (!success)
				throw new TfDboServiceException("Delete<TfSpaceViewDbo> failed");

			scope.Complete();
		}
	}

	public async Task AddSpaceViewPreset(
		Guid spaceViewId,
		TfSpaceViewPreset preset)
	{
		if (preset.Id == Guid.Empty)
			preset.Id = Guid.NewGuid();
		var existingSpaceView = _dboManager.Get<TfSpaceViewDbo>(spaceViewId);
		if (existingSpaceView == null)
			throw new TfValidationException("spaceViewId", "SpaceView not found.");


		if (string.IsNullOrWhiteSpace(preset.Name) && string.IsNullOrWhiteSpace(preset.Icon))
			throw new TfValidationException(nameof(TfSpaceViewPreset.Name),
				"Name is required when icon is not provided.");

		var presets = new List<TfSpaceViewPreset>();
		if (!String.IsNullOrWhiteSpace(existingSpaceView.PresetsJson) && existingSpaceView.PresetsJson != "[]")
			presets = JsonSerializer.Deserialize<List<TfSpaceViewPreset>>(existingSpaceView.PresetsJson) ?? new();

		if (preset.ParentId is not null)
		{
			TfSpaceViewPreset? parentNode = ModelHelpers.GetPresetById(presets, preset.ParentId.Value);
			if (parentNode is not null)
				parentNode.Presets.Add(preset);
		}
		else
		{
			presets.Add(preset);
		}

		existingSpaceView.PresetsJson =
			JsonSerializer.Serialize(presets);

		var success = _dboManager.Update<TfSpaceViewDbo>(
			existingSpaceView,
			nameof(TfSpaceViewDbo.PresetsJson));

		if (!success)
			throw new TfDboServiceException("Update<TfSpaceViewDbo> failed.");

		var spaceView = GetSpaceView(spaceViewId);
		await PublishEventWithScopeAsync(new TfSpaceViewUpdatedEvent(spaceView));
	}

	public async Task UpdateSpaceViewPreset(
		Guid spaceViewId,
		TfSpaceViewPreset preset)
	{
		var existingSpaceView = _dboManager.Get<TfSpaceViewDbo>(spaceViewId);
		if (existingSpaceView == null)
			throw new TfValidationException("spaceViewId", "SpaceView not found.");

		if (string.IsNullOrWhiteSpace(preset.Name) && string.IsNullOrWhiteSpace(preset.Icon))
			throw new TfValidationException(nameof(TfSpaceViewPreset.Name),
				"Name is required when icon is not provided.");

		var presets = new List<TfSpaceViewPreset>();
		if (!String.IsNullOrWhiteSpace(existingSpaceView.PresetsJson) && existingSpaceView.PresetsJson != "[]")
			presets = JsonSerializer.Deserialize<List<TfSpaceViewPreset>>(existingSpaceView.PresetsJson) ?? new();

		var currentPreset = presets.GetPresetById(preset.Id);
		if (currentPreset is null)
			throw new TfValidationException("preset", "Preset with this ID was not found.");
		Guid? currentParentId = currentPreset.ParentId;


		currentPreset.Name = preset.Name;
		currentPreset.Icon = preset.Icon;
		currentPreset.ParentId = preset.ParentId;
		currentPreset.Color = preset.Color;
		currentPreset.Search = preset.Search;
		currentPreset.Filters = preset.Filters;
		currentPreset.SortOrders = preset.SortOrders;

		if (currentParentId != preset.ParentId)
		{
			TfSpaceViewPreset? currentParent = null;
			TfSpaceViewPreset? newParent = null;
			if (currentParentId.HasValue) currentParent = presets.GetPresetById(currentParentId.Value);
			if (preset.ParentId.HasValue) newParent = presets.GetPresetById(preset.ParentId.Value);

			if (currentParent is not null)
			{
				var findIndex = currentParent.Presets.FindIndex(x => x.Id == preset.Id);
				if (findIndex > -1) currentParent.Presets.RemoveAt(findIndex);
			}
			else
			{
				var findIndex = presets.FindIndex(x => x.Id == preset.Id);
				if (findIndex > -1) presets.RemoveAt(findIndex);
			}

			if (newParent is not null)
			{
				newParent.Presets.Add(currentPreset);
			}
			else
			{
				presets.Add(currentPreset);
			}
		}


		existingSpaceView.PresetsJson =
			JsonSerializer.Serialize(presets);

		var success = _dboManager.Update<TfSpaceViewDbo>(
			existingSpaceView,
			nameof(TfSpaceViewDbo.PresetsJson));

		if (!success)
			throw new TfDboServiceException("Update<TfSpaceViewDbo> failed.");

		var spaceView = GetSpaceView(spaceViewId);
		await PublishEventWithScopeAsync(new TfSpaceViewUpdatedEvent(spaceView));
	}

	public async Task CopySpaceViewPreset(
		Guid spaceViewId,
		Guid presetId)
	{
		var existingSpaceView = _dboManager.Get<TfSpaceViewDbo>(spaceViewId);
		if (existingSpaceView == null)
			throw new TfValidationException("spaceViewId", "SpaceView not found.");		
		var presets = new List<TfSpaceViewPreset>();
		if (!String.IsNullOrWhiteSpace(existingSpaceView.PresetsJson) && existingSpaceView.PresetsJson != "[]")
			presets = JsonSerializer.Deserialize<List<TfSpaceViewPreset>>(existingSpaceView.PresetsJson) ?? new();

		var sourcePreset = presets.GetPresetById(presetId);
		if (sourcePreset is null)
			throw new TfValidationException("preset", "Preset with this ID was not found.");		
		
		if (sourcePreset.ParentId is not null)
		{
			var parent = presets.GetPresetById(sourcePreset.ParentId.Value);
			if (parent is null) return;

			var sourceIndex = parent.Presets.FindIndex(x => x.Id == sourcePreset.Id);
			if (sourceIndex > -1)
			{
				parent.Presets.Insert(sourceIndex + 1, _copyPreset(sourcePreset, parent.Id));
			}
		}
		else
		{
			var sourceIndex = presets.FindIndex(x => x.Id == sourcePreset.Id);
			if (sourceIndex > -1)
			{
				presets.Insert(sourceIndex + 1, _copyPreset(sourcePreset, null));
			}
		}		
		
		existingSpaceView.PresetsJson =
			JsonSerializer.Serialize(presets);

		var success = _dboManager.Update<TfSpaceViewDbo>(
			existingSpaceView,
			nameof(TfSpaceViewDbo.PresetsJson));

		if (!success)
			throw new TfDboServiceException("Update<TfSpaceViewDbo> failed.");

		var spaceView = GetSpaceView(spaceViewId);
		await PublishEventWithScopeAsync(new TfSpaceViewUpdatedEvent(spaceView));		
	}
	
	public async Task MoveSpaceViewPreset(
		Guid spaceViewId,
		Guid presetId,
		bool isUp)
	{
		var existingSpaceView = _dboManager.Get<TfSpaceViewDbo>(spaceViewId);
		if (existingSpaceView == null)
			throw new TfValidationException("spaceViewId", "SpaceView not found.");		
		var presets = new List<TfSpaceViewPreset>();
		if (!String.IsNullOrWhiteSpace(existingSpaceView.PresetsJson) && existingSpaceView.PresetsJson != "[]")
			presets = JsonSerializer.Deserialize<List<TfSpaceViewPreset>>(existingSpaceView.PresetsJson) ?? new();

		var sourcePreset = presets.GetPresetById(presetId);
		if (sourcePreset is null)
			throw new TfValidationException("preset", "Preset with this ID was not found.");		
		
		presets = _movePreset(presets, presetId, isUp);
		
		existingSpaceView.PresetsJson =
			JsonSerializer.Serialize(presets);

		var success = _dboManager.Update<TfSpaceViewDbo>(
			existingSpaceView,
			nameof(TfSpaceViewDbo.PresetsJson));

		if (!success)
			throw new TfDboServiceException("Update<TfSpaceViewDbo> failed.");

		var spaceView = GetSpaceView(spaceViewId);
		await PublishEventWithScopeAsync(new TfSpaceViewUpdatedEvent(spaceView));		
	}	
	
	public async Task RemoveSpaceViewPreset(
		Guid spaceViewId,
		Guid presetId)
	{
		var existingSpaceView = _dboManager.Get<TfSpaceViewDbo>(spaceViewId);
		if (existingSpaceView == null)
			throw new TfValidationException("spaceViewId", "SpaceView not found.");		
		var presets = new List<TfSpaceViewPreset>();
		if (!String.IsNullOrWhiteSpace(existingSpaceView.PresetsJson) && existingSpaceView.PresetsJson != "[]")
			presets = JsonSerializer.Deserialize<List<TfSpaceViewPreset>>(existingSpaceView.PresetsJson) ?? new();

		var sourcePreset = presets.GetPresetById(presetId);
		if (sourcePreset is null)
			throw new TfValidationException("preset", "Preset with this ID was not found.");		
		
		presets = _removePreset(presets, presetId);
		existingSpaceView.PresetsJson =
			JsonSerializer.Serialize(presets);

		var success = _dboManager.Update<TfSpaceViewDbo>(
			existingSpaceView,
			nameof(TfSpaceViewDbo.PresetsJson));

		if (!success)
			throw new TfDboServiceException("Update<TfSpaceViewDbo> failed.");

		var spaceView = GetSpaceView(spaceViewId);
		await PublishEventWithScopeAsync(new TfSpaceViewUpdatedEvent(spaceView));		
	}	
	
	private TfSpaceView? ConvertDboToModel(
		TfSpaceViewDbo? dbo)
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
			SettingsJson = dbo.SettingsJson,
			DatasetId = dbo.SpaceDataId,
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
			SettingsJson = model.SettingsJson,
			SpaceDataId = model.DatasetId,
			PresetsJson = JsonSerializer.Serialize(model.Presets ?? new List<TfSpaceViewPreset>())
		};
	}

	#region << private >>
	private TfSpaceViewPreset _copyPreset(TfSpaceViewPreset item,Guid? parentId = null)
	{
		var newItem = item with { Id = Guid.NewGuid(), ParentId = parentId };
		var newNodes = new List<TfSpaceViewPreset>();
		foreach (var node in item.Presets)
		{
			newNodes.Add(_copyPreset(node, newItem.Id));
		}
		newItem.Presets = newNodes;
		return newItem;
	}	
	
	private List<TfSpaceViewPreset> _movePreset(List<TfSpaceViewPreset> nodes, Guid nodeId, bool isUp)
	{
		if (nodes.Count == 0) return nodes;

		var nodeIndex = nodes.FindIndex(x => x.Id == nodeId);
		if (nodeIndex > -1)
		{
			var list = nodes.Where(x => x.Id != nodeId).ToList();
			var newIndex = isUp ? nodeIndex - 1 : nodeIndex + 1;
			if (newIndex < 0 || newIndex > nodes.Count - 1) return nodes;

			list.Insert(newIndex, nodes[nodeIndex]);
			return list;
		}

		foreach (var item in nodes)
		{
			item.Presets = _movePreset(item.Presets, nodeId, isUp);
		}

		return nodes;
	}	
	
	private List<TfSpaceViewPreset> _removePreset(List<TfSpaceViewPreset> nodes, Guid nodeId)
	{
		if (nodes.Count == 0) return nodes;
		if (nodes.Any(x => x.Id == nodeId))
		{
			return nodes.Where(x => x.Id != nodeId).ToList();
		}

		foreach (var item in nodes)
		{
			item.Presets = _removePreset(item.Presets, nodeId);
		}

		return nodes;
	}	
	#endregion
	
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
				return new ValidationResult(new[]
				{
					new ValidationFailure("",
						"The space view is null.")
				});

			return this.Validate(spaceView, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}


		public ValidationResult ValidateUpdate(
			TfSpaceView spaceView)
		{
			if (spaceView == null)
				return new ValidationResult(new[]
				{
					new ValidationFailure("",
						"The space view is null.")
				});

			return this.Validate(spaceView, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}

		public ValidationResult ValidateDelete(
			TfSpaceView spaceView)
		{
			if (spaceView == null)
				return new ValidationResult(new[]
				{
					new ValidationFailure("",
						"The space view with specified identifier is not found.")
				});

			return this.Validate(spaceView, options =>
			{
				options.IncludeRuleSets("delete");
			});
		}
	}

	#endregion
}