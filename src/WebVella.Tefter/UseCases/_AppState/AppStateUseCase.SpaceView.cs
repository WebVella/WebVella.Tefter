namespace WebVella.Tefter.UseCases.AppState;
internal partial class AppStateUseCase
{
	internal Task<(TfAppState, TfAuxDataState)> InitSpaceViewAsync(
		IServiceProvider serviceProvider,
		TucUser currentUser,
		TfRouteState routeState,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
	{
		if (newAppState.Space is null)
		{
			newAppState = newAppState with
			{
				SpaceViewList = new(),
				SpaceView = null,
				AvailableColumnTypes = new(),
				SpaceViewColumns = new(),
				SpaceViewData = null,
				SelectedDataRows = new()
			};
			return Task.FromResult((newAppState,newAuxDataState));
		}

		//SpaceViewList

		if (newAppState.Space?.Id != oldAppState.Space?.Id)
			newAppState = newAppState with { SpaceViewList = GetSpaceViewList(routeState.SpaceId.Value) };

		//Space View
		if (routeState.SpaceViewId is not null)
		{
			int defaultPageSize = TfConstants.PageSize;
			if (currentUser.Settings.PageSize is not null) defaultPageSize = currentUser.Settings.PageSize.Value;
			newAppState = newAppState with
			{
				SpaceView = GetSpaceView(routeState.SpaceViewId.Value),
				SpaceViewColumns = GetViewColumns(routeState.SpaceViewId.Value),
				SpaceViewPage = routeState.Page ?? 1,
				SpaceViewPageSize = routeState.PageSize ?? defaultPageSize,
				SpaceViewSearch = routeState.Search,
				SpaceViewFilters = routeState.Filters,
				SpaceViewSorts = routeState.Sorts,
			};
			if (newAppState.SpaceView is not null && newAppState.SpaceView.SpaceDataId.HasValue)
			{
				TucSpaceViewPreset preset = null;
				if (routeState.SpaceViewPresetId is not null)
					preset = newAppState.SpaceView.Presets.GetPresetById(routeState.SpaceViewPresetId.Value);

				var viewData = GetSpaceDataDataTable(
							spaceDataId: newAppState.SpaceView.SpaceDataId.Value,
							presetFilters: preset is not null ? preset.Filters : null,
							presetSorts: preset is not null ? preset.SortOrders : null,
							userFilters: newAppState.SpaceViewFilters,
							userSorts: newAppState.SpaceViewSorts,
							search: newAppState.SpaceViewSearch,
							page: newAppState.SpaceViewPage,
							pageSize: newAppState.SpaceViewPageSize
						);
				newAppState = newAppState with
				{
					SpaceViewData = viewData,
					SpaceViewPage = viewData?.QueryInfo.Page ?? newAppState.SpaceViewPage,
				};
			}
			else
			{
				newAppState = newAppState with { SpaceViewData = null };
			}
			if (newAppState.SpaceView is not null)
			{
				//Aux Data Hook
				var compContext = new TucViewColumnComponentContext()
				{
					Hash = newAppState.Hash,
					DataTable = newAppState.SpaceViewData,
					Mode = TucComponentMode.Display, //ignored here
					SpaceViewId = newAppState.SpaceView.Id,
					EditContext = null, //ignored here
					ValidationMessageStore = null, //ignored here
					RowIndex = 0,///ignored here
					CustomOptionsJson = null, //set in column loop
					DataMapping = null,//set in column loop
					QueryName = null,//set in column loop
					SpaceViewColumnId = Guid.Empty, //set in column loop
				};
				foreach (TucSpaceViewColumn column in newAppState.SpaceViewColumns)
				{
					if (column.ComponentType is not null
						&& column.ComponentType.GetInterface(nameof(ITucAuxDataUseComponent)) != null)
					{
						compContext.SpaceViewColumnId = column.Id;
						compContext.CustomOptionsJson = column.CustomOptionsJson;
						compContext.DataMapping = column.DataMapping;
						compContext.QueryName = column.QueryName;
						var component = (ITucAuxDataUseComponent)Activator.CreateInstance(column.ComponentType, compContext);
						component.OnSpaceViewStateInited(
								serviceProvider: serviceProvider,
								currentUser: currentUser,
								routeState: routeState,
								newAppState: newAppState,
								oldAppState: oldAppState,
								newAuxDataState: newAuxDataState,
								oldAuxDataState: oldAuxDataState
						);
					}
				}

				//Addon Components
				var addonComponents = GetAddonComponents(null).Where(x => x.Region == TfScreenRegion.SpaceViewToolbarActions
					|| x.Region == TfScreenRegion.SpaceViewSelectorActions).ToList();
				newAppState = newAppState with { SpaceViewAddonComponents = addonComponents };
			}
		}
		else
		{
			newAppState = newAppState with
			{
				SpaceView = null,
				SpaceViewColumns = new(),
				SpaceViewData = null,
				SpaceViewPage = 0,
				SpaceViewPageSize = TfConstants.PageSize,
				SpaceViewSearch = null,
				SpaceViewFilters = new(),
				SpaceViewSorts = new(),
				SelectedDataRows = new()
			};
		}
		newAppState = newAppState with { AvailableColumnTypes = GetAvailableSpaceViewColumnTypes() };

		//SelectedDataRows
		if (oldAppState.SpaceView?.Id != newAppState.SpaceView?.Id)
			newAppState = newAppState with { SelectedDataRows = new() };


		return Task.FromResult((newAppState,newAuxDataState));
	}
	internal TucSpaceView GetSpaceView(Guid viewId)
	{
		var serviceResult = _spaceManager.GetSpaceView(viewId);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpaceView failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
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
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		if (serviceResult.Value is null) return new();

		return serviceResult.Value.Select(x => new TucSpaceView(x)).ToList();
	}
	internal Result<Tuple<TucSpaceView, TucSpaceData>> CreateSpaceViewWithForm(TucSpaceView view)
	{
		var serviceResult = _spaceManager.CreateSpaceView(view.ToModelExtended(), view.DataSetType == TucSpaceViewDataSetType.New);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("CreateSpaceView failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}

		var spaceDataResult = _spaceManager.GetSpaceData(serviceResult.Value.SpaceDataId);
		if (spaceDataResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpaceData failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}

		var spaceView = new TucSpaceView(serviceResult.Value);
		var spaceData = new TucSpaceData(spaceDataResult.Value);
		return Result.Ok(new Tuple<TucSpaceView, TucSpaceData>(spaceView, spaceData));
	}

	internal Result<Tuple<TucSpaceView, TucSpaceData>> UpdateSpaceViewWithForm(TucSpaceView view)
	{
		var serviceResult = _spaceManager.UpdateSpaceView(view.ToModelExtended(), view.DataSetType == TucSpaceViewDataSetType.New);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("UpdateSpaceView failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}

		var spaceDataResult = _spaceManager.GetSpaceData(serviceResult.Value.SpaceDataId);
		if (spaceDataResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpaceData failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}

		var spaceView = new TucSpaceView(serviceResult.Value);
		var spaceData = new TucSpaceData(spaceDataResult.Value);
		return Result.Ok(new Tuple<TucSpaceView, TucSpaceData>(spaceView, spaceData));
	}

	internal Result DeleteSpaceView(Guid viewId)
	{
		var tfResult = _spaceManager.DeleteSpaceView(viewId);
		if (tfResult.IsFailed) return Result.Fail(new Error("DeleteSpaceView failed").CausedBy(tfResult.Errors));

		return Result.Ok();
	}

	internal Task<List<TucSpaceView>> GetAllSpaceViews()
	{

		var serviceResult = _spaceManager.GetAllSpaceViews();
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetAllSpaceViews failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.FromResult(new List<TucSpaceView>());
		}
		if (serviceResult.Value is null) return Task.FromResult(new List<TucSpaceView>());
		return Task.FromResult(serviceResult.Value.Select(x => new TucSpaceView(x)).ToList());

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
				toastValidationMessage: "Invalid Data",
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
				toastValidationMessage: "Invalid Data",
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
		var selectedType = availableTypes.FirstOrDefault(x => x.Id == column.ColumnType?.Id);
		if (selectedType is null) return Result.Fail("Column selected type not found");
		var result = _spaceManager.CreateSpaceViewColumn(column.ToModel(selectedType));

		if (result.IsFailed) return Result.Fail(new Error("CreateSpaceViewColumn failed").CausedBy(result.Errors));
		return Result.Ok(GetViewColumns(column.SpaceViewId));
	}

	internal Result<List<TucSpaceViewColumn>> UpdateSpaceViewColumnWithForm(TucSpaceViewColumn column)
	{
		var availableTypes = _spaceManager.GetAvailableSpaceViewColumnTypes().Value;
		var selectedType = availableTypes.FirstOrDefault(x => x.Id == column.ColumnType?.Id);
		if (selectedType is null) return Result.Fail("Column selected type not found");
		var result = _spaceManager.UpdateSpaceViewColumn(column.ToModel(selectedType));

		if (result.IsFailed) return Result.Fail(new Error("CreateSpaceViewColumn failed").CausedBy(result.Errors));
		return Result.Ok(GetViewColumns(column.SpaceViewId));
	}

	internal Result<List<TucSpaceViewColumn>> RemoveSpaceViewColumn(Guid columnId)
	{
		if (columnId == Guid.Empty) return Result.Fail("columnId is required");
		var column = GetViewColumn(columnId);
		if (column is null) return Result.Fail("column not found");
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
	internal Result<TucSpaceView> UpdateSpaceViewPresets(Guid viewId, List<TucSpaceViewPreset> presets)
	{
		var updateResult = _spaceManager.UpdateSpaceViewPresets(viewId, presets.Select(x => x.ToModel()).ToList());
		if (updateResult.IsFailed) return Result.Fail(new Error("UpdateSpaceViewPresets failed").CausedBy(updateResult.Errors));
		return Result.Ok(GetSpaceView(viewId));

	}

	internal List<TucSpaceViewColumnType> GetAvailableSpaceViewColumnTypes()
	{
		var serviceResult = _spaceManager.GetAvailableSpaceViewColumnTypes();
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetAvailableSpaceViewColumnTypes failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
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
