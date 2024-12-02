namespace WebVella.Tefter.UseCases.AppState;
internal partial class AppStateUseCase
{
	internal Task<(TfAppState, TfAuxDataState)> InitSpaceDataAsync(IServiceProvider serviceProvider,
		TucUser currentUser,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
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
			return Task.FromResult((newAppState, newAuxDataState));
		}
		if (newAppState.Route.SpaceDataId is not null)
		{
			newAppState = newAppState with { SpaceData = GetSpaceData(newAppState.Route.SpaceDataId.Value) };

			//Space Data data
			if (newAppState.Route.ThirdNode == RouteDataThirdNode.Data)
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


		return Task.FromResult((newAppState, newAuxDataState));
	}
	internal virtual TucSpaceData GetSpaceData(Guid spaceDataId)
	{
		var serviceResult = _spaceManager.GetSpaceData(spaceDataId);
		if (serviceResult.IsFailed)
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
		if (serviceResult.Value is null) return null;

		return new TucSpaceData(serviceResult.Value);
	}

	internal virtual List<TucSpaceData> GetSpaceDataList(Guid spaceId)
	{
		var serviceResult = _spaceManager.GetSpaceDataList(spaceId);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpaceDataList failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		if (serviceResult.Value is null) return new();

		return serviceResult.Value.Select(x => new TucSpaceData(x)).ToList();
	}

	internal virtual Result DeleteSpaceData(Guid dataId)
	{
		var tfResult = _spaceManager.DeleteSpaceData(dataId);
		if (tfResult.IsFailed) return Result.Fail(new Error("DeleteSpaceView failed").CausedBy(tfResult.Errors));

		return Result.Ok();
	}


	internal virtual Result<TucSpaceData> CreateSpaceDataWithForm(TucSpaceData form)
	{
		TfSpace space = null;
		TfDataProvider dataprovider = null;
		#region << Validate>>
		var validationErrors = new List<ValidationError>();
		//args
		if (String.IsNullOrWhiteSpace(form.Name)) validationErrors.Add(new ValidationError(nameof(form.Name), "name is required"));
		if (form.SpaceId == Guid.Empty) validationErrors.Add(new ValidationError(nameof(form.SpaceId), "space is required"));
		if (form.DataProviderId == Guid.Empty) validationErrors.Add(new ValidationError(nameof(form.DataProviderId), "dataprovider is required"));

		//Space
		var spaceResult = _spaceManager.GetSpace(form.SpaceId);
		if (spaceResult.IsFailed) return Result.Fail(new Error("GetSpace failed").CausedBy(spaceResult.Errors));
		if (spaceResult.Value is null) validationErrors.Add(new ValidationError(nameof(form.SpaceId), "space is not found"));
		space = spaceResult.Value;

		//DataProvider
		if (form.DataProviderId != Guid.Empty)
		{
			var providerResult = _dataProviderManager.GetProvider(form.DataProviderId);
			if (providerResult.IsFailed) return Result.Fail(new Error("GetProvider failed").CausedBy(providerResult.Errors));
			if (providerResult.Value is null) validationErrors.Add(new ValidationError(nameof(form.DataProviderId), "data provider is not found"));
			dataprovider = providerResult.Value;
		}

		if (validationErrors.Count > 0)
			return Result.Fail(validationErrors);

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

		var tfResult = _spaceManager.CreateSpaceData(spaceDataObj);
		if (tfResult.IsFailed) return Result.Fail(new Error("CreateSpaceData failed").CausedBy(tfResult.Errors));
		if (tfResult.Value is null) return Result.Fail("CreateSpaceData failed to return value");

		//Should commit transaction
		return Result.Ok(new TucSpaceData(tfResult.Value));
	}

	internal virtual Result<TucSpaceData> UpdateSpaceDataWithForm(TucSpaceData form)
	{
		TfSpace space = null;
		TfSpaceData spaceData = null;
		TfDataProvider dataprovider = null;
		#region << Validate>>
		var validationErrors = new List<ValidationError>();
		//args
		if (form.Id == Guid.Empty) validationErrors.Add(new ValidationError(nameof(form.Id), "required"));
		if (String.IsNullOrWhiteSpace(form.Name)) validationErrors.Add(new ValidationError(nameof(form.Name), "required"));
		if (form.SpaceId == Guid.Empty) validationErrors.Add(new ValidationError(nameof(form.SpaceId), "required"));
		if (form.DataProviderId == Guid.Empty) validationErrors.Add(new ValidationError(nameof(form.DataProviderId), "required"));

		//Space
		var spaceResult = _spaceManager.GetSpace(form.SpaceId);
		if (spaceResult.IsFailed) return Result.Fail(new Error("GetSpace failed").CausedBy(spaceResult.Errors));
		if (spaceResult.Value is null) validationErrors.Add(new ValidationError(nameof(form.SpaceId), "space is not found"));
		space = spaceResult.Value;

		//DataProvider
		if (form.DataProviderId != Guid.Empty)
		{
			var providerResult = _dataProviderManager.GetProvider(form.DataProviderId);
			if (providerResult.IsFailed) return Result.Fail(new Error("GetProvider failed").CausedBy(providerResult.Errors));
			if (providerResult.Value is null) validationErrors.Add(new ValidationError(nameof(form.DataProviderId), "data provider is not found"));
			dataprovider = providerResult.Value;
		}

		//SpaceData
		var spaceDataResult = _spaceManager.GetSpaceData(form.Id);
		if (spaceDataResult.IsFailed) return Result.Fail(new Error("GetSpaceData failed").CausedBy(spaceDataResult.Errors));
		if (spaceDataResult.Value is null) validationErrors.Add(new ValidationError(nameof(form.Id), "dataset is not found"));
		spaceData = spaceDataResult.Value;



		if (validationErrors.Count > 0)
			return Result.Fail(validationErrors);

		#endregion
		spaceData.Name = form.Name;
		spaceData.DataProviderId = form.DataProviderId;

		var tfResult = _spaceManager.UpdateSpaceData(spaceData);
		if (tfResult.IsFailed) return Result.Fail(new Error("UpdateSpaceData failed").CausedBy(tfResult.Errors));
		if (tfResult.Value is null) return Result.Fail("UpdateSpaceData failed to return value");

		//Should commit transaction
		return Result.Ok(new TucSpaceData(tfResult.Value));
	}

	internal virtual Result<TucSpaceData> UpdateSpaceDataColumns(Guid spaceDataId, List<string> columns)
	{
		if (spaceDataId == Guid.Empty) return Result.Fail("spaceDataId is required");
		var spaceData = GetSpaceData(spaceDataId);
		if (spaceData is null) return Result.Fail("spaceData not found");
		spaceData.Columns = columns;
		var model = spaceData.ToModel();
		var updateResult = _spaceManager.UpdateSpaceData(spaceData.ToModel());
		if (updateResult.IsFailed) return Result.Fail(new Error("UpdateSpaceData failed").CausedBy(updateResult.Errors));

		return Result.Ok(new TucSpaceData(updateResult.Value));

	}

	internal virtual Result<TucSpaceData> UpdateSpaceDataFilters(Guid spaceDataId, List<TucFilterBase> filters)
	{
		if (spaceDataId == Guid.Empty) return Result.Fail("spaceDataId is required");
		var spaceData = GetSpaceData(spaceDataId);
		if (spaceData is null) return Result.Fail("spaceData not found");
		spaceData.Filters = filters;
		var model = spaceData.ToModel();
		var updateResult = _spaceManager.UpdateSpaceData(spaceData.ToModel());
		if (updateResult.IsFailed) return Result.Fail(new Error("UpdateSpaceData failed").CausedBy(updateResult.Errors));

		return Result.Ok(new TucSpaceData(updateResult.Value));

	}

	internal virtual Result<TucSpaceData> UpdateSpaceDataSorts(Guid spaceDataId, List<TucSort> sorts)
	{
		if (spaceDataId == Guid.Empty) return Result.Fail("spaceDataId is required");
		var spaceData = GetSpaceData(spaceDataId);
		if (spaceData is null) return Result.Fail("spaceData not found");
		spaceData.SortOrders = sorts;
		var model = spaceData.ToModel();
		var updateResult = _spaceManager.UpdateSpaceData(spaceData.ToModel());
		if (updateResult.IsFailed) return Result.Fail(new Error("UpdateSpaceData failed").CausedBy(updateResult.Errors));

		return Result.Ok(new TucSpaceData(updateResult.Value));

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
		if (spaceDataId == Guid.Empty)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail("spaceDataId not provided"),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		var spaceData = GetSpaceData(spaceDataId);
		if (spaceData is null)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail("Space Data is not found"),
				toastErrorMessage: "Space Data is not found",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Space Data is not found",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}

		List<TfFilterBase> presetFiltersSM = null;
		List<TfSort> presetSortsSM = null;
		List<TfFilterBase> userFiltersSM = null;
		List<TfSort> userSortsSM = null;
		if (presetFilters is not null) presetFiltersSM = presetFilters.Select(x => TucFilterBase.ToModel(x)).ToList();
		if (presetSorts is not null) presetSortsSM = presetSorts.Select(x => x.ToModel()).ToList();
		if (userFilters is not null) userFiltersSM = userFilters.Select(x => TucFilterBase.ToModel(x)).ToList();
		if (userSorts is not null) userSortsSM = userSorts.Select(x => x.ToModel()).ToList();

		var serviceResult = _dataManager.QuerySpaceData(
			spaceDataId: spaceDataId,
			presetFilters: presetFiltersSM,
			presetSorts: presetSortsSM,
			userFilters: userFiltersSM,
			userSorts: userSortsSM,
			search: search,
			page: page,
			pageSize: pageSize
		);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("QuerySpaceData failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}

		return serviceResult.Value;
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
		if (spaceDataId == Guid.Empty)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail("spaceDataId not provided"),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		var spaceData = GetSpaceData(spaceDataId);
		if (spaceData is null)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail("Space Data is not found"),
				toastErrorMessage: "Space Data is not found",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Space Data is not found",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}

		List<TfFilterBase> presetFiltersSM = null;
		List<TfSort> presetSortsSM = null;
		List<TfFilterBase> userFiltersSM = null;
		List<TfSort> userSortsSM = null;
		if (presetFilters is not null) presetFiltersSM = presetFilters.Select(x => TucFilterBase.ToModel(x)).ToList();
		if (presetSorts is not null) presetSortsSM = presetSorts.Select(x => x.ToModel()).ToList();
		if (userFilters is not null) userFiltersSM = userFilters.Select(x => TucFilterBase.ToModel(x)).ToList();
		if (userSorts is not null) userSortsSM = userSorts.Select(x => x.ToModel()).ToList();

		var serviceResult = _dataManager.QuerySpaceData(
			spaceDataId: spaceDataId,
			presetFilters: presetFiltersSM,
			presetSorts: presetSortsSM,
			userFilters: userFiltersSM,
			userSorts: userSortsSM,
			search: search,
			page: page,
			pageSize: pageSize,
			noRows:false,
			returnOnlyTfIds:true
		);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("QuerySpaceData failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		var result = new List<Guid>();
		for (int i = 0; i < serviceResult.Value.Rows.Count; i++)
		{
			result.Add((Guid)serviceResult.Value.Rows[i][TfConstants.TEFTER_ITEM_ID_PROP_NAME]);
		}
		return result;
	}

	internal virtual Result<TfDataTable> SaveDataDataTable(TfDataTable dt)
	{
		var saveResult = _dataManager.SaveDataTable(dt);
		if (saveResult.IsFailed) return Result.Fail(new Error("SaveDataTable failed").CausedBy(saveResult.Errors));
		return Result.Ok(saveResult.Value);
	}

	internal virtual Result DeleteSpaceDataRows(Guid spaceDataId, List<Guid> tfIdList)
	{
		if (spaceDataId == Guid.Empty)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail("spaceDataId not provided"),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		var spaceData = GetSpaceData(spaceDataId);
		if (spaceData is null)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail("Space Data is not found"),
				toastErrorMessage: "Space Data is not found",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Space Data is not found",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		var dataProviderResult = _dataProviderManager.GetProvider(spaceData.DataProviderId);
		if (dataProviderResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetProvider failed").CausedBy(dataProviderResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}

		foreach (var tfId in tfIdList)
		{
			var result = _dataManager.DeleteDataProviderRowByTfId(dataProviderResult.Value, tfId);
			if (result.IsFailed) return Result.Fail("Deleting a record failed");
		}
		return Result.Ok();
	}

	//Data provider
	internal virtual List<TucDataProvider> GetDataProviderList()
	{
		var serviceResult = _dataProviderManager.GetProviders();
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetProviders failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		if (serviceResult.Value is null) return new();

		return serviceResult.Value.Select(x => new TucDataProvider(x)).ToList();
	}
}
