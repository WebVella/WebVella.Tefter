namespace WebVella.Tefter.UseCases.AppState;
internal partial class AppStateUseCase
{
	internal async Task<(TfAppState, TfAuxDataState)> InitSpaceViewAsync(
		IServiceProvider serviceProvider,
		TucUser currentUser,
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
			return (newAppState,newAuxDataState);
		}

		if (newAppState.Route.SpaceViewId is not null)
		{
			int defaultPageSize = TfConstants.PageSize;
			if (currentUser.Settings.PageSize is not null) defaultPageSize = currentUser.Settings.PageSize.Value;
			newAppState = newAppState with
			{
				SpaceView = GetSpaceView(newAppState.Route.SpaceViewId.Value),
				SpaceViewColumns = GetViewColumns(newAppState.Route.SpaceViewId.Value),
				SpaceViewFilters = newAppState.Route.Filters,
				SpaceViewSorts = newAppState.Route.Sorts,
			};
			if (newAppState.SpaceView is not null && newAppState.SpaceView.SpaceDataId.HasValue)
			{
				TucSpaceViewPreset preset = null;
				if (newAppState.Route.SpaceViewPresetId is not null)
					preset = newAppState.SpaceView.Presets.GetPresetById(newAppState.Route.SpaceViewPresetId.Value);

				var viewData = GetSpaceDataDataTable(
							spaceDataId: newAppState.SpaceView.SpaceDataId.Value,
							presetFilters: preset is not null ? preset.Filters : null,
							presetSorts: preset is not null ? preset.SortOrders : null,
							userFilters: newAppState.SpaceViewFilters,
							userSorts: newAppState.SpaceViewSorts,
							search: newAppState.Route.Search,
							page: newAppState.Route.Page,
							pageSize: newAppState.Route.PageSize
						);
				newAppState = newAppState with
				{
					SpaceViewData = viewData,
					SpaceData = GetSpaceData(newAppState.SpaceView.SpaceDataId.Value),
					Route = newAppState.Route with { Page = viewData.QueryInfo.Page }
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
						await component.OnAppStateInit(
								serviceProvider: serviceProvider,
								currentUser: currentUser,
								newAppState: newAppState,
								oldAppState: oldAppState,
								newAuxDataState: newAuxDataState,
								oldAuxDataState: oldAuxDataState
						);
					}
				}
			}
		}
		else
		{
			newAppState = newAppState with
			{
				SpaceView = null,
				SpaceViewColumns = new(),
				SpaceViewData = null,
				SpaceViewFilters = new(),
				SpaceViewSorts = new(),
				SelectedDataRows = new()
			};
		}
		newAppState = newAppState with { AvailableColumnTypes = GetAvailableSpaceViewColumnTypes() };

		//SelectedDataRows
		if (oldAppState.SpaceView?.Id != newAppState.SpaceView?.Id)
			newAppState = newAppState with { SelectedDataRows = new() };


		return (newAppState,newAuxDataState);
	}
	internal virtual TucSpaceView GetSpaceView(Guid viewId)
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
	internal virtual List<TucSpaceView> GetSpaceViewList(Guid spaceId)
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
	internal virtual Result<Tuple<TucSpaceView, TucSpaceData>> CreateSpaceViewWithForm(TucSpaceView view)
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
	internal virtual Result<Tuple<TucSpaceView, TucSpaceData>> UpdateSpaceViewWithForm(TucSpaceView view)
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
	internal virtual Result DeleteSpaceView(Guid viewId)
	{
		var tfResult = _spaceManager.DeleteSpaceView(viewId);
		if (tfResult.IsFailed) return Result.Fail(new Error("DeleteSpaceView failed").CausedBy(tfResult.Errors));

		return Result.Ok();
	}
	internal virtual Task<List<TucSpaceView>> GetAllSpaceViews()
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
	internal virtual TucSpaceViewColumn GetViewColumn(Guid columnId)
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

	internal virtual List<TucSpaceViewColumn> GetViewColumns(Guid viewId)
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

	internal virtual Result<List<TucSpaceViewColumn>> CreateSpaceViewColumnWithForm(TucSpaceViewColumn column)
	{
		var availableTypes = _spaceManager.GetAvailableSpaceViewColumnTypes().Value;
		var selectedType = availableTypes.FirstOrDefault(x => x.Id == column.ColumnType?.Id);
		if (selectedType is null) return Result.Fail("Column selected type not found");
		var result = _spaceManager.CreateSpaceViewColumn(column.ToModel(selectedType));

		if (result.IsFailed) return Result.Fail(new Error("CreateSpaceViewColumn failed").CausedBy(result.Errors));
		return Result.Ok(GetViewColumns(column.SpaceViewId));
	}

	internal virtual Result<List<TucSpaceViewColumn>> UpdateSpaceViewColumnWithForm(TucSpaceViewColumn column)
	{
		var availableTypes = _spaceManager.GetAvailableSpaceViewColumnTypes().Value;
		var selectedType = availableTypes.FirstOrDefault(x => x.Id == column.ColumnType?.Id);
		if (selectedType is null) return Result.Fail("Column selected type not found");
		var result = _spaceManager.UpdateSpaceViewColumn(column.ToModel(selectedType));

		if (result.IsFailed) return Result.Fail(new Error("CreateSpaceViewColumn failed").CausedBy(result.Errors));
		return Result.Ok(GetViewColumns(column.SpaceViewId));
	}

	internal virtual Result<List<TucSpaceViewColumn>> RemoveSpaceViewColumn(Guid columnId)
	{
		if (columnId == Guid.Empty) return Result.Fail("columnId is required");
		var column = GetViewColumn(columnId);
		if (column is null) return Result.Fail("column not found");
		var updateResult = _spaceManager.DeleteSpaceViewColumn(columnId);
		if (updateResult.IsFailed) return Result.Fail(new Error("DeleteSpaceViewColumn failed").CausedBy(updateResult.Errors));
		return Result.Ok(GetViewColumns(column.SpaceViewId));
	}

	internal virtual Result<List<TucSpaceViewColumn>> MoveSpaceViewColumn(Guid viewId, Guid columnId, bool isUp)
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
	internal virtual Result<TucSpaceView> UpdateSpaceViewPresets(Guid viewId, List<TucSpaceViewPreset> presets)
	{
		var updateResult = _spaceManager.UpdateSpaceViewPresets(viewId, presets.Select(x => x.ToModel()).ToList());
		if (updateResult.IsFailed) return Result.Fail(new Error("UpdateSpaceViewPresets failed").CausedBy(updateResult.Errors));
		return Result.Ok(GetSpaceView(viewId));

	}

	internal virtual List<TucSpaceViewColumnType> GetAvailableSpaceViewColumnTypes()
	{
		var serviceResult = _metaProvider.GetSpaceViewColumnTypesMeta();

		return serviceResult.Select(x => new TucSpaceViewColumnType(x.Instance)).ToList();

	}

}
