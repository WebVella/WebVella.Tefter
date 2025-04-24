namespace WebVella.Tefter.UseCases.AppState;
internal partial class AppStateUseCase
{
	internal async Task<(TfAppState, TfAuxDataState)> InitSpaceDataAsync(
		IServiceProvider serviceProvider,
		TucUser currentUser,
		TfAppState newAppState,
		TfAppState oldAppState,
		TfAuxDataState newAuxDataState,
		TfAuxDataState oldAuxDataState)
	{
		if (newAppState.Space is null)
		{
			newAppState = newAppState with
			{
				SpaceData = null,
				SpaceDataList = new(),
				AllDataProviders = new(),
				SpaceDataData = null,
				SpaceDataPage = 1,
				SpaceDataPageSize = TfConstants.PageSize,
				SpaceDataSearch = null
			};
			return (newAppState, newAuxDataState);
		}
		if (newAppState.Route.SpaceDataId is not null)
		{
			newAppState = newAppState with { SpaceData = GetSpaceData(newAppState.Route.SpaceDataId.Value) };

			//if provider is not found then we init space data as null
			var provider = await GetDataProviderAsync(newAppState.SpaceData.DataProviderId);
			if (provider is null)
			{
				newAppState = newAppState with { SpaceData = null };
			}
			//Space Data data init
			else if (newAppState.Route.ThirdNode == RouteDataThirdNode.Data)
			{
				var viewData = GetSpaceDataDataTable(
							spaceDataId: newAppState.SpaceData.Id,
							userFilters: null,
							userSorts: null,
							search: newAppState.Route.Search,
							page: newAppState.Route.Page,
							pageSize: newAppState.Route.PageSize ?? TfConstants.PageSize
						);
				newAppState = newAppState with
				{
					SpaceDataData = viewData,
					SpaceDataPage = viewData?.QueryInfo.Page ?? (newAppState.Route.Page ?? 1),
					SpaceDataPageSize = newAppState.Route.PageSize ?? TfConstants.PageSize,
					SpaceDataSearch = newAppState.Route.Search
				};
			}

		}
		else
		{
			newAppState = newAppState with { SpaceData = null };
		}
		newAppState = newAppState with { AllDataProviders = GetDataProviderList() };

		return (newAppState, newAuxDataState);
	}
	internal virtual TucSpaceData GetSpaceData(
		Guid spaceDataId)
	{
		try
		{
			var spaceData = _tfService.GetSpaceData(spaceDataId);
			if (spaceData is null)
				return null;

			return new TucSpaceData(spaceData);
		}
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceException(
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

	internal virtual List<TucSpaceData> GetSpaceDataList(
		Guid spaceId)
	{
		try
		{
			var spaceDataList = _tfService.GetSpaceDataList(spaceId);
			if (spaceDataList is null)
				return new();

			return spaceDataList
				.Select(x => new TucSpaceData(x))
				.ToList();
		}
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceException(
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

	internal virtual void DeleteSpaceData(
		Guid dataId)
	{
		_tfService.DeleteSpaceData(dataId);
	}


	internal virtual TucSpaceData CreateSpaceDataWithForm(
		TucSpaceData form)
	{
		TfSpace space = null;
		TfDataProvider dataprovider = null;

		#region << Validate >>

		var valEx = new TfValidationException();
		//args
		if (String.IsNullOrWhiteSpace(form.Name))
			valEx.AddValidationError(nameof(form.Name), "name is required");

		if (form.SpaceId == Guid.Empty)
			valEx.AddValidationError(nameof(form.SpaceId), "space is required");

		if (form.DataProviderId == Guid.Empty)
			valEx.AddValidationError(nameof(form.DataProviderId), "dataprovider is required");

		//Space
		space = _tfService.GetSpace(form.SpaceId);
		if (space is null)
			valEx.AddValidationError(nameof(form.SpaceId), "space is not found");

		//DataProvider
		if (form.DataProviderId != Guid.Empty)
		{
			dataprovider = _tfService.GetDataProvider(form.DataProviderId);
			if (dataprovider is null)
				valEx.AddValidationError(nameof(form.DataProviderId), "data provider is not found");
		}

		valEx.ThrowIfContainsErrors();

		#endregion

		var spaceDataObj = new TfSpaceData()
		{
			Id = Guid.NewGuid(),
			Name = form.Name,
			Filters = new(),//filters will not be added at this point
			Columns = new(), // columns will be added later
			DataProviderId = dataprovider.Id,
			SpaceId = space.Id,
			Position = 1 //position is overrided in the creation
		};

		var spaceData = _tfService.CreateSpaceData(spaceDataObj);

		return new TucSpaceData(spaceData);
	}

	internal virtual TucSpaceData UpdateSpaceDataWithForm(
		TucSpaceData form)
	{
		TfSpace space = null;
		TfSpaceData spaceData = null;
		TfDataProvider dataprovider = null;

		#region << Validate>>

		var valEx = new TfValidationException();

		//args
		if (form.Id == Guid.Empty)
			valEx.AddValidationError(nameof(form.Id), "required");

		if (String.IsNullOrWhiteSpace(form.Name))
			valEx.AddValidationError(nameof(form.Name), "required");

		if (form.SpaceId == Guid.Empty)
			valEx.AddValidationError(nameof(form.SpaceId), "required");

		if (form.DataProviderId == Guid.Empty)
			valEx.AddValidationError(nameof(form.DataProviderId), "required");

		//Space
		space = _tfService.GetSpace(form.SpaceId);
		if (space is null)
			valEx.AddValidationError(nameof(form.SpaceId), "space is not found");

		//DataProvider
		if (form.DataProviderId != Guid.Empty)
		{
			dataprovider = _tfService.GetDataProvider(form.DataProviderId);
			if (dataprovider is null)
				valEx.AddValidationError(nameof(form.DataProviderId), "data provider is not found");
		}

		//SpaceData
		spaceData = _tfService.GetSpaceData(form.Id);
		if (spaceData is null)
			valEx.AddValidationError(nameof(form.Id), "dataset is not found");

		valEx.ThrowIfContainsErrors();

		#endregion

		spaceData.Name = form.Name;
		spaceData.DataProviderId = form.DataProviderId;

		var updatedSpaceData = _tfService.UpdateSpaceData(spaceData);

		//Should commit transaction
		return new TucSpaceData(updatedSpaceData);
	}

	internal virtual TucSpaceData UpdateSpaceDataColumns(
		Guid spaceDataId,
		List<string> columns)
	{
		if (spaceDataId == Guid.Empty)
			new TfException("spaceDataId is required");

		var spaceData = GetSpaceData(spaceDataId);
		if (spaceData is null)
			new TfException("spaceData not found");

		spaceData.Columns = columns;

		var updatedSpaceData = _tfService.UpdateSpaceData(spaceData.ToModel());

		return new TucSpaceData(updatedSpaceData);

	}

	internal virtual TucSpaceData UpdateSpaceDataFilters(
		Guid spaceDataId,
		List<TucFilterBase> filters)
	{
		if (spaceDataId == Guid.Empty)
			throw new TfException("spaceDataId is required");

		var spaceData = GetSpaceData(spaceDataId);
		if (spaceData is null)
			throw new TfException("spaceData not found");

		spaceData.Filters = filters;

		var updatedSpaceData = _tfService.UpdateSpaceData(spaceData.ToModel());

		return new TucSpaceData(updatedSpaceData);

	}

	internal virtual TucSpaceData UpdateSpaceDataSorts(
		Guid spaceDataId,
		List<TucSort> sorts)
	{
		if (spaceDataId == Guid.Empty)
			throw new TfException("spaceDataId is required");

		var spaceData = GetSpaceData(spaceDataId);
		if (spaceData is null)
			throw new TfException("spaceData not found");

		spaceData.SortOrders = sorts;

		var updatedSpaceData = _tfService.UpdateSpaceData(spaceData.ToModel());

		return new TucSpaceData(updatedSpaceData);

	}

	//Data
	internal virtual TfDataTable GetSpaceDataDataTable(
		Guid spaceDataId,
		List<TucFilterBase> presetFilters = null,
		List<TucSort> presetSorts = null,
		List<TucFilterBase> userFilters = null,
		List<TucSort> userSorts = null,
		string search = null,
		int? page = null,
		int? pageSize = null)
	{
		try
		{
			if (spaceDataId == Guid.Empty)
				throw new TfException("spaceDataId not provided");

			var spaceData = GetSpaceData(spaceDataId);
			if (spaceData is null)
				throw new TfException("Space Dataset is not found");

			List<TfFilterBase> presetFiltersSM = null;
			List<TfSort> presetSortsSM = null;
			List<TfFilterBase> userFiltersSM = null;
			List<TfSort> userSortsSM = null;
			if (presetFilters is not null) presetFiltersSM = presetFilters.Select(x => TucFilterBase.ToModel(x)).ToList();
			if (presetSorts is not null) presetSortsSM = presetSorts.Select(x => x.ToModel()).ToList();
			if (userFilters is not null) userFiltersSM = userFilters.Select(x => TucFilterBase.ToModel(x)).ToList();
			if (userSorts is not null) userSortsSM = userSorts.Select(x => x.ToModel()).ToList();

			return _tfService.QuerySpaceData(
				spaceDataId: spaceDataId,
				presetFilters: presetFiltersSM,
				presetSorts: presetSortsSM,
				userFilters: userFiltersSM,
				userSorts: userSortsSM,
				search: search,
				page: page,
				pageSize: pageSize
			);
		}
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceException(
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

	internal virtual List<Guid> GetSpaceDataIdList(
		Guid spaceDataId,
		List<TucFilterBase> presetFilters = null,
		List<TucSort> presetSorts = null,
		List<TucFilterBase> userFilters = null,
		List<TucSort> userSorts = null,
		string search = null,
		int? page = null,
		int? pageSize = null)
	{
		try
		{
			if (spaceDataId == Guid.Empty)
				throw new TfException("spaceDataId not provided");

			var spaceData = GetSpaceData(spaceDataId);
			if (spaceData is null)
				throw new TfException("Space Dataset is not found");

			List<TfFilterBase> presetFiltersSM = null;
			List<TfSort> presetSortsSM = null;
			List<TfFilterBase> userFiltersSM = null;
			List<TfSort> userSortsSM = null;
			if (presetFilters is not null) presetFiltersSM = presetFilters.Select(x => TucFilterBase.ToModel(x)).ToList();
			if (presetSorts is not null) presetSortsSM = presetSorts.Select(x => x.ToModel()).ToList();
			if (userFilters is not null) userFiltersSM = userFilters.Select(x => TucFilterBase.ToModel(x)).ToList();
			if (userSorts is not null) userSortsSM = userSorts.Select(x => x.ToModel()).ToList();

			var dt = _tfService.QuerySpaceData(
				spaceDataId: spaceDataId,
				presetFilters: presetFiltersSM,
				presetSorts: presetSortsSM,
				userFilters: userFiltersSM,
				userSorts: userSortsSM,
				search: search,
				page: page,
				pageSize: pageSize,
				noRows: false,
				returnOnlyTfIds: true
			);

			var result = new List<Guid>();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				result.Add((Guid)dt.Rows[i][TfConstants.TEFTER_ITEM_ID_PROP_NAME]);
			}
			return result;
		}
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceException(
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

	internal virtual TfDataTable SaveDataDataTable(
		TfDataTable dt)
	{
		return _tfService.SaveDataTable(dt);
	}

	internal virtual void DeleteSpaceDataRows(
		Guid spaceDataId,
		List<Guid> tfIdList)
	{
		try
		{
			if (spaceDataId == Guid.Empty)
				throw new TfException("spaceDataId not provided");

			var spaceData = GetSpaceData(spaceDataId);
			if (spaceData is null)
				throw new TfException("Space Dataset is not found");

			var dataProvider = _tfService.GetDataProvider(spaceData.DataProviderId);
			if (dataProvider is null)
				throw new TfException("GetProvider failed");

			foreach (var tfId in tfIdList)
			{
				_tfService.DeleteDataProviderRowByTfId(dataProvider, tfId);
			}
		}
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceException(
					exception: ex,
					toastErrorMessage: "Unexpected Error",
					toastValidationMessage: "Invalid Data",
					notificationErrorTitle: "Unexpected Error",
					toastService: _toastService,
					messageService: _messageService
				);
		}
	}

	//Data provider
	internal virtual List<TucDataProvider> GetDataProviderList()
	{
		try
		{
			return _tfService.GetDataProviders()
				.Select(x => new TucDataProvider(x))
				.ToList();
		}
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceException(
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
}
