﻿namespace WebVella.Tefter.UseCases.AppStart;
internal partial class AppStateUseCase
{
	internal Task<TfAppState> InitSpaceViewAsync(TucUser currentUser, TfRouteState routeState, TfAppState result)
	{
		if (routeState.SpaceId is null)
		{
			result = result with
			{
				SpaceViewList = new(),
				SpaceView = null,
				AvailableColumnTypes = new(),
				SpaceViewColumns = new()
			};
			return Task.FromResult(result);
		}

		//SpaceViewList
		if (
			result.SpaceViewList.Count == 0
			|| !result.SpaceViewList.Any(x => x.SpaceId == routeState.SpaceId)
			|| (routeState.SpaceViewId is not null && !result.SpaceViewList.Any(x => x.Id == routeState.SpaceViewId))
			)
			result = result with { SpaceViewList = GetSpaceViewList(routeState.SpaceId.Value) };

		//Space View
		if (routeState.SpaceViewId is not null)
		{
			result = result with
			{
				SpaceView = GetSpaceView(routeState.SpaceViewId.Value),
				SpaceViewColumns = GetViewColumns(routeState.SpaceViewId.Value)
			};
		}
		else
		{
			result = result with { SpaceView = null, SpaceViewColumns = new() };
		}
		result = result with { AvailableColumnTypes = GetAvailableSpaceViewColumnTypes() };

		return Task.FromResult(result);
	}
	internal TucSpaceView GetSpaceView(Guid viewId)
	{
		var serviceResult = _spaceManager.GetSpaceView(viewId);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpaceView failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		if (serviceResult.Value is null) return null;

		return new TucSpaceView(serviceResult.Value);
	}
	internal List<TucSpaceView> GetSpaceViewList(Guid spaceId)
	{
		var serviceResult = _spaceManager.GetSpaceViewsList(spaceId);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpaceViewsList failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		if (serviceResult.Value is null) return new();

		return serviceResult.Value.Select(x => new TucSpaceView(x)).ToList();
	}
	internal Result<TucSpaceView> CreateSpaceViewWithForm(TucSpaceView view)
	{
		//TODO RUMEN: big part of this needs to be created as a service and be in transaction

		TfSpace space = null;
		TfSpaceData spaceData = null;
		TfSpaceView spaceView = null;
		TfDataProvider dataprovider = null;
		#region << Validate>>
		var validationErrors = new List<ValidationError>();
		//args
		if (String.IsNullOrWhiteSpace(view.Name)) validationErrors.Add(new ValidationError(nameof(view.Name), "required"));
		if (view.SpaceId == Guid.Empty) validationErrors.Add(new ValidationError(nameof(view.SpaceId), "required"));
		if (view.DataSetType == TucSpaceViewDataSetType.New)
		{
			if (String.IsNullOrWhiteSpace(view.NewSpaceDataName)) validationErrors.Add(new ValidationError(nameof(view.NewSpaceDataName), "required"));
			if (view.DataProviderId is null) validationErrors.Add(new ValidationError(nameof(view.DataProviderId), "required"));
		}
		else if (view.DataSetType == TucSpaceViewDataSetType.Existing)
			if (view.SpaceDataId is null) validationErrors.Add(new ValidationError(nameof(view.SpaceDataId), "required"));

		//Space
		var spaceResult = _spaceManager.GetSpace(view.SpaceId);
		if (spaceResult.IsFailed) return Result.Fail(new Error("GetSpace failed").CausedBy(spaceResult.Errors));
		if (spaceResult.Value is null) validationErrors.Add(new ValidationError(nameof(view.SpaceId), "space is not found"));
		space = spaceResult.Value;

		//SpaceData
		if (view.SpaceDataId is not null)
		{
			var spaceDataResult = _spaceManager.GetSpaceData(view.SpaceDataId.Value);
			if (spaceDataResult.IsFailed) return Result.Fail(new Error("GetSpaceData failed").CausedBy(spaceDataResult.Errors));
			if (spaceDataResult.Value is null) validationErrors.Add(new ValidationError(nameof(view.SpaceDataId), "dataset is not found"));
			spaceData = spaceDataResult.Value;
		}

		//DataProvider
		Guid? dataProviderId = null;
		if (view.DataProviderId is not null) dataProviderId = view.DataProviderId.Value;
		else if (spaceData is not null) dataProviderId = spaceData.DataProviderId;
		if (dataProviderId is not null)
		{
			var providerResult = _dataProviderManager.GetProvider(dataProviderId.Value);
			if (providerResult.IsFailed) return Result.Fail(new Error("GetProvider failed").CausedBy(providerResult.Errors));
			if (providerResult.Value is null) validationErrors.Add(new ValidationError(nameof(dataProviderId), "data provider is not found"));
			dataprovider = providerResult.Value;
		}

		if (validationErrors.Count > 0)
			return Result.Fail(validationErrors);

		#endregion

		//Should start transaction
		#region << create space data if needed >>
		if (spaceData is null)
		{
			List<string> selectedColumns = new();
			//system columns are always selected so we should not add them in the space data
			if (view.AddProviderColumns && view.AddSharedColumns)
			{
				//all columns are requested from the provider, so send empty column list, which will apply newly added columns
				//to the provider dynamically
			}
			else if (view.AddProviderColumns) selectedColumns.AddRange(dataprovider.Columns.Select(x => x.DbName).ToList());
			else if (view.AddSharedColumns) selectedColumns.AddRange(dataprovider.SharedColumns.Select(x => x.DbName).ToList());

			var spaceDataObj = new TfSpaceData()
			{
				Id = Guid.NewGuid(),
				Name = view.NewSpaceDataName,
				Filters = new(),//filters will not be added at this point
				Columns = selectedColumns,
				DataProviderId = dataprovider.Id,
				SpaceId = space.Id,
				Position = 1 //position is overrided in the creation
			};

			var tfResult = _spaceManager.CreateSpaceData(spaceDataObj);
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
				Name = view.Name,
				Position = 1,//will be overrided later
				SpaceDataId = spaceData.Id,
				SpaceId = space.Id,
				Type = view.Type.ConvertSafeToEnum<TucSpaceViewType, TfSpaceViewType>()
			};
			var tfResult = _spaceManager.CreateSpaceView(spaceViewObj);
			if (tfResult.IsFailed) return Result.Fail(new Error("CreateSpaceView failed").CausedBy(tfResult.Errors));
			if (tfResult.Value is null) return Result.Fail("CreateSpaceView failed to return value");
			spaceView = tfResult.Value;
		}
		#endregion

		#region << create view columns>>
		{
			var availableTypes = _spaceManager.GetAvailableSpaceViewColumnTypes().Value;
			var columnsToCreate = new List<TfSpaceViewColumn>();
			short position = 1;
			if (view.DataSetType == TucSpaceViewDataSetType.New)
			{
				if (view.AddProviderColumns)
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
							SelectedAddonId = null
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
				if (view.AddSharedColumns)
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
							FullTypeName = null,
							SelectedAddonId = null
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
				if (view.AddSystemColumns)
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
							SelectedAddonId = null
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
			else if (view.DataSetType == TucSpaceViewDataSetType.Existing)
			{
				if (view.AddDatasetColumns)
				{
					foreach (var dbName in spaceData.Columns)
					{
						DatabaseColumnType? dbType = dataprovider.Columns.FirstOrDefault(x => x.DbName == dbName)?.DbType;
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
							SelectedAddonId = null
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
				var tfResult = _spaceManager.CreateSpaceViewColumn(tfColumn);
				if (tfResult.IsFailed) return Result.Fail(new Error("CreateSpaceViewColumn failed").CausedBy(tfResult.Errors));
				if (tfResult.Value is null) return Result.Fail("CreateSpaceViewColumn failed to return value");
			}
		}
		#endregion
		//Should commit transaction

		return Result.Ok(new TucSpaceView(spaceView));
	}

	internal Result<TucSpaceView> UpdateSpaceViewWithForm(TucSpaceView view)
	{
		//TODO RUMEN: big part of this needs to be created as a service and be in transaction
		TfSpace space = null;
		TfSpaceData spaceData = null;
		TfSpaceView spaceView = null;
		TfDataProvider dataprovider = null;
		#region << Validate>>
		var validationErrors = new List<ValidationError>();
		//args
		if (String.IsNullOrWhiteSpace(view.Name)) validationErrors.Add(new ValidationError(nameof(view.Name), "required"));
		if (view.SpaceId == Guid.Empty) validationErrors.Add(new ValidationError(nameof(view.SpaceId), "required"));
		if (view.DataSetType == TucSpaceViewDataSetType.New)
		{
			if (String.IsNullOrWhiteSpace(view.NewSpaceDataName)) validationErrors.Add(new ValidationError(nameof(view.NewSpaceDataName), "required"));
			if (view.DataProviderId is null) validationErrors.Add(new ValidationError(nameof(view.DataProviderId), "required"));
		}
		else if (view.DataSetType == TucSpaceViewDataSetType.Existing)
			if (view.SpaceDataId is null) validationErrors.Add(new ValidationError(nameof(view.SpaceDataId), "required"));

		//Space
		var spaceResult = _spaceManager.GetSpace(view.SpaceId);
		if (spaceResult.IsFailed) return Result.Fail(new Error("GetSpace failed").CausedBy(spaceResult.Errors));
		if (spaceResult.Value is null) validationErrors.Add(new ValidationError(nameof(view.SpaceId), "space is not found"));
		space = spaceResult.Value;

		//DataProvider
		if (view.DataProviderId is not null)
		{
			var providerResult = _dataProviderManager.GetProvider(view.DataProviderId.Value);
			if (providerResult.IsFailed) return Result.Fail(new Error("GetProvider failed").CausedBy(providerResult.Errors));
			if (providerResult.Value is null) validationErrors.Add(new ValidationError(nameof(view.DataProviderId), "data provider is not found"));
			dataprovider = providerResult.Value;
		}

		//SpaceData
		if (view.SpaceDataId is not null)
		{
			var spaceDataResult = _spaceManager.GetSpaceData(view.SpaceDataId.Value);
			if (spaceDataResult.IsFailed) return Result.Fail(new Error("GetSpaceData failed").CausedBy(spaceDataResult.Errors));
			if (spaceDataResult.Value is null) validationErrors.Add(new ValidationError(nameof(view.SpaceDataId), "dataset is not found"));
			spaceData = spaceDataResult.Value;
		}

		if (validationErrors.Count > 0)
			return Result.Fail(validationErrors);

		#endregion

		//Should start transaction
		#region << create space data if needed >>
		if (spaceData is null)
		{
			List<string> selectedColumns = new();
			//system columns are always selected so we should not add them in the space data
			if (view.AddProviderColumns && view.AddSharedColumns)
			{
				//all columns are requested from the provider, so send empty column list, which will apply newly added columns
				//to the provider dynamically
			}
			else if (view.AddProviderColumns) selectedColumns.AddRange(dataprovider.Columns.Select(x => x.DbName).ToList());
			else if (view.AddSharedColumns) selectedColumns.AddRange(dataprovider.SharedColumns.Select(x => x.DbName).ToList());

			var spaceDataObj = new TfSpaceData()
			{
				Id = Guid.NewGuid(),
				Name = view.NewSpaceDataName,
				Filters = new(),//filters will not be added at this point
				Columns = selectedColumns,
				DataProviderId = dataprovider.Id,
				SpaceId = space.Id,
				Position = 1 //position is overrided in the creation
			};

			var tfResult = _spaceManager.CreateSpaceData(spaceDataObj);
			if (tfResult.IsFailed) return Result.Fail(new Error("CreateSpaceData failed").CausedBy(tfResult.Errors));
			if (tfResult.Value is null) return Result.Fail("CreateSpaceData failed to return value");
			spaceData = tfResult.Value;
		}
		#endregion

		#region << update view>>
		{
			var spaceViewObj = new TfSpaceView()
			{
				Id = view.Id,
				Name = view.Name,
				Position = 1,//will be overrided later
				SpaceDataId = spaceData.Id,
				SpaceId = space.Id,
				Type = view.Type.ConvertSafeToEnum<TucSpaceViewType, TfSpaceViewType>()
			};
			var tfResult = _spaceManager.UpdateSpaceView(spaceViewObj);
			if (tfResult.IsFailed) return Result.Fail(new Error("UpdateSpaceView failed").CausedBy(tfResult.Errors));
			if (tfResult.Value is null) return Result.Fail("UpdateSpaceView failed to return value");
			spaceView = tfResult.Value;
		}
		#endregion

		//Should commit transaction

		return Result.Ok(new TucSpaceView(spaceView));
	}

	internal Result DeleteSpaceView(Guid viewId)
	{
		var tfResult = _spaceManager.DeleteSpaceView(viewId);
		if (tfResult.IsFailed) return Result.Fail(new Error("DeleteSpaceView failed").CausedBy(tfResult.Errors));

		return Result.Ok();
	}

	internal TfDataTable GetSpaceViewData(
		Guid spaceDataId,
		List<TucFilterBase> additionalFilters = null,
		List<TucSort> sortOverrides = null,
		string search = null,
		int? page = null,
		int? pageSize = null)
	{
		if (spaceDataId == Guid.Empty)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail("spaceDataId not provided"),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		List<TfFilterBase> filters = null;
		List<TfSort> sorts = null;
		if (additionalFilters is not null) filters = additionalFilters.Select(x => TucFilterBase.ToModel(x)).ToList();
		if (sortOverrides is not null) sorts = sortOverrides.Select(x => x.ToModel()).ToList();

		var serviceResult = _dataManager.QuerySpaceData(
			spaceDataId: spaceDataId,
			additionalFilters: filters,
			sortOverrides: sorts,
			search: search,
			page: page,
			pageSize: pageSize
		);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("QuerySpaceData failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}

		return serviceResult.Value;
	}

	//View columns
	internal TucSpaceViewColumn GetViewColumn(Guid columnId)
	{
		var serviceResult = _spaceManager.GetSpaceViewColumn(columnId);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpaceViewColumn failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return new();
		}
		if (serviceResult.Value is null) return new();

		return new TucSpaceViewColumn(serviceResult.Value);

	}

	internal List<TucSpaceViewColumn> GetViewColumns(Guid viewId)
	{
		var serviceResult = _spaceManager.GetSpaceViewColumnsList(viewId);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpaceViewColumnsList failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return new();
		}
		if (serviceResult.Value is null) return new();

		return serviceResult.Value.Select(x => new TucSpaceViewColumn(x)).ToList();

	}

	internal Result<List<TucSpaceViewColumn>> CreateSpaceViewColumnWithForm(TucSpaceViewColumn column)
	{
		var availableTypes = _spaceManager.GetAvailableSpaceViewColumnTypes().Value;
		var selectedType = availableTypes.FirstOrDefault(x => x.Id == column.ColumnType.Id);
		if (selectedType is null) return Result.Fail("Column selected type not found");
		var result = _spaceManager.CreateSpaceViewColumn(column.ToModel(selectedType));

		if (result.IsFailed) return Result.Fail(new Error("CreateSpaceViewColumn failed").CausedBy(result.Errors));
		return Result.Ok(GetViewColumns(column.SpaceViewId));
	}

	internal Result<List<TucSpaceViewColumn>> UpdateSpaceViewColumnWithForm(TucSpaceViewColumn column)
	{
		var availableTypes = _spaceManager.GetAvailableSpaceViewColumnTypes().Value;
		var selectedType = availableTypes.FirstOrDefault(x => x.Id == column.ColumnType.Id);
		if (selectedType is null) return Result.Fail("Column selected type not found");
		var result = _spaceManager.UpdateSpaceViewColumn(column.ToModel(selectedType));

		if (result.IsFailed) return Result.Fail(new Error("CreateSpaceViewColumn failed").CausedBy(result.Errors));
		return Result.Ok(GetViewColumns(column.SpaceViewId));
	}

	internal Result<List<TucSpaceViewColumn>> RemoveSpaceViewColumn(Guid columnId)
	{
		if (columnId == Guid.Empty) return Result.Fail("columnId is required");
		var column = GetViewColumn(columnId);
		if(column is null) return Result.Fail("column not found");
		var updateResult = _spaceManager.DeleteSpaceViewColumn(columnId);
		if (updateResult.IsFailed) return Result.Fail(new Error("DeleteSpaceViewColumn failed").CausedBy(updateResult.Errors));
		return Result.Ok(GetViewColumns(column.SpaceViewId));
	}

	internal Result<List<TucSpaceViewColumn>> MoveSpaceViewColumn(Guid viewId, Guid columnId, bool isUp)
	{
		if (columnId == Guid.Empty) return Result.Fail("columnId is required");
		Result updateResult = null;
		if (isUp)
			updateResult = _spaceManager.MoveSpaceViewColumnUp(columnId);
		else
			updateResult = _spaceManager.MoveSpaceViewColumnDown(columnId);

		if (updateResult.IsFailed) return Result.Fail(new Error("MoveSpaceViewColumn failed").CausedBy(updateResult.Errors));
		return Result.Ok(GetViewColumns(viewId));

	}

	internal List<TucSpaceViewColumnType> GetAvailableSpaceViewColumnTypes()
	{
		var serviceResult = _spaceManager.GetAvailableSpaceViewColumnTypes();
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetAvailableSpaceViewColumnTypes failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return new();
		}
		if (serviceResult.Value is null) return new();

		return serviceResult.Value.Select(x => new TucSpaceViewColumnType(x)).ToList();

	}

}
