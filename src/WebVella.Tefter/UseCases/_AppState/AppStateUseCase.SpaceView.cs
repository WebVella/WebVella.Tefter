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
	internal virtual TucSpaceView GetSpaceView(
		Guid viewId)
	{
		try
		{
			var spaceView = _spaceManager.GetSpaceView(viewId);
			if (spaceView is null) 
				return null;

			return new TucSpaceView(spaceView);
		}
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceResult(
				exception: ex,
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
	}

	internal virtual List<TucSpaceView> GetSpaceViewList(
		Guid spaceId)
	{
		try
		{
			return _spaceManager.GetSpaceViewsList(spaceId)
				.Select(x => new TucSpaceView(x))
				.ToList();
		}
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceResult(
				exception: ex,
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
	}
	internal virtual Tuple<TucSpaceView, TucSpaceData> CreateSpaceViewWithForm(
		TucSpaceView view)
	{
		try
		{
			var spaceView = _spaceManager.CreateSpaceView(
				view.ToModelExtended(), 
				view.DataSetType == TucSpaceViewDataSetType.New);

			var spaceData = _spaceManager.GetSpaceData(spaceView.SpaceDataId);

			return new Tuple<TucSpaceView, TucSpaceData>(
				new TucSpaceView(spaceView),
				new TucSpaceData(spaceData));
		}
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceResult(
				exception: ex,
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
	}
	internal virtual Tuple<TucSpaceView, TucSpaceData> UpdateSpaceViewWithForm(
		TucSpaceView view)
	{
		try
		{
			var spaceView = _spaceManager.UpdateSpaceView(
				view.ToModelExtended(), 
				view.DataSetType == TucSpaceViewDataSetType.New);
			
			var spaceData = _spaceManager.GetSpaceData(spaceView.SpaceDataId);

			return new Tuple<TucSpaceView, TucSpaceData>(
				new TucSpaceView(spaceView),
				new TucSpaceData(spaceData));
		}
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceResult(
				exception: ex,
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
	}

	internal virtual void DeleteSpaceView(
		Guid viewId)
	{
		_spaceManager.DeleteSpaceView(viewId);
	}

	internal virtual Task<List<TucSpaceView>> GetAllSpaceViews()
	{
		try
		{
			var allSpaceViews = _spaceManager.GetAllSpaceViews();
			if (allSpaceViews is null) 
				return Task.FromResult(new List<TucSpaceView>());

			return Task.FromResult(allSpaceViews
				.Select(x => new TucSpaceView(x))
				.ToList());
		}
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceResult(
				exception: ex,
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.FromResult(new List<TucSpaceView>());
		}
	}

	//View columns
	internal virtual TucSpaceViewColumn GetViewColumn(
		Guid columnId)
	{
		try
		{
			var spaceViewColumn = _spaceManager.GetSpaceViewColumn(columnId);
			if (spaceViewColumn is null)
				return new TucSpaceViewColumn();

			return new TucSpaceViewColumn(spaceViewColumn);
		}
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceResult(
				exception: ex,
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return new TucSpaceViewColumn();
		}
	}

	internal virtual List<TucSpaceViewColumn> GetViewColumns(
		Guid viewId)
	{
		try
		{
			var columnList = _spaceManager.GetSpaceViewColumnsList(viewId);
			if (columnList is null) 
				return new List<TucSpaceViewColumn>();

			return columnList.Select(x => new TucSpaceViewColumn(x)).ToList();
		}
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceResult(
				exception: ex,
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return new List<TucSpaceViewColumn>();
		}

	}

	internal virtual List<TucSpaceViewColumn> CreateSpaceViewColumnWithForm(
		TucSpaceViewColumn column)
	{
		var availableTypes = _spaceManager.GetAvailableSpaceViewColumnTypes();
		var selectedType = availableTypes.FirstOrDefault(x => x.Id == column.ColumnType?.Id);
		if (selectedType is null)
			throw new TfException("Column selected type is not found");

		_spaceManager.CreateSpaceViewColumn(column.ToModel(selectedType));
		
		return GetViewColumns(column.SpaceViewId);
	}

	internal virtual List<TucSpaceViewColumn> UpdateSpaceViewColumnWithForm(
		TucSpaceViewColumn column)
	{
		var availableTypes = _spaceManager.GetAvailableSpaceViewColumnTypes();

		var selectedType = availableTypes.FirstOrDefault(x => x.Id == column.ColumnType?.Id);
		if (selectedType is null)
			throw new TfException("Column selected type is not found");

		_spaceManager.UpdateSpaceViewColumn(column.ToModel(selectedType));

		return GetViewColumns(column.SpaceViewId);
	}

	internal virtual List<TucSpaceViewColumn> RemoveSpaceViewColumn(
		Guid columnId)
	{
		if (columnId == Guid.Empty)
			throw new TfException("Column ID is not specified");

		var column = GetViewColumn(columnId);

		if (column is null)
			throw new TfException("Column is not found");

		_spaceManager.DeleteSpaceViewColumn(columnId);
		
		return GetViewColumns(column.SpaceViewId);
	}

	internal virtual List<TucSpaceViewColumn> MoveSpaceViewColumn(
		Guid viewId, 
		Guid columnId, 
		bool isUp)
	{
		if (columnId == Guid.Empty)
			throw new TfException("Column ID is not specified");
		;
		if (isUp)
			_spaceManager.MoveSpaceViewColumnUp(columnId);
		else
			_spaceManager.MoveSpaceViewColumnDown(columnId);

		return GetViewColumns(viewId);

	}

	internal virtual TucSpaceView UpdateSpaceViewPresets(
		Guid viewId,
		List<TucSpaceViewPreset> presets)
	{
		_spaceManager.UpdateSpaceViewPresets(viewId, presets.Select(x => x.ToModel()).ToList());
		return GetSpaceView(viewId);

	}

	internal virtual List<TucSpaceViewColumnType> GetAvailableSpaceViewColumnTypes()
	{
		var serviceResult = _metaProvider.GetSpaceViewColumnTypesMeta();
		return serviceResult.Select(x => new TucSpaceViewColumnType(x.Instance)).ToList();

	}

}
