namespace WebVella.Tefter.UseCases.AppStart;
internal partial class AppStateUseCase
{
	internal Task<TfAppState> InitSpaceDataAsync(TucUser currentUser, TfRouteState routeState, TfAppState result)
	{
		if (routeState.SpaceId is null)
		{
			result = result with { SpaceData = null, SpaceDataList = new(), AllDataProviders = new() };
			return Task.FromResult(result);
		}

		//SpaceDataList
		if (result.SpaceDataList.Count == 0
			|| !result.SpaceDataList.Any(x => x.SpaceId == routeState.SpaceId)
			|| (routeState.SpaceDataId is not null && !result.SpaceDataList.Any(x => x.Id == routeState.SpaceDataId))
			)
			result = result with { SpaceDataList = GetSpaceDataList(routeState.SpaceId.Value) };
		//SpaceData
		if (routeState.SpaceDataId is not null)
		{
			result = result with { SpaceData = GetSpaceData(routeState.SpaceDataId.Value) };

		}
		else
		{
			result = result with { SpaceData = null };
		}

		result = result with { AllDataProviders = GetDataProviderList() };


		return Task.FromResult(result);
	}
	internal TucSpaceData GetSpaceData(Guid spaceDataId)
	{
		var serviceResult = _spaceManager.GetSpaceData(spaceDataId);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpaceData failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		if (serviceResult.Value is null) return null;

		return new TucSpaceData(serviceResult.Value);
	}

	internal List<TucSpaceData> GetSpaceDataList(Guid spaceId)
	{
		var serviceResult = _spaceManager.GetSpaceDataList(spaceId);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpaceDataList failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		if (serviceResult.Value is null) return new();

		return serviceResult.Value.Select(x => new TucSpaceData(x)).ToList();
	}

	internal Result<TucSpaceData> CreateSpaceDataWithForm(TucSpaceData form)
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

	internal Result<TucSpaceData> UpdateSpaceDataWithForm(TucSpaceData form)
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

	internal Result<TucSpaceData> AddColumnToSpaceData(Guid spaceDataId, string columnDbName)
	{
		if (spaceDataId == Guid.Empty) return Result.Fail("spaceDataId is required");
		if (String.IsNullOrWhiteSpace(columnDbName)) return Result.Fail("columnDbName is required");
		var spaceData = GetSpaceData(spaceDataId);
		if (spaceData is null) return Result.Fail("spaceData not found");
		if (spaceData.Columns.Contains(columnDbName)) return Result.Ok(spaceData);

		spaceData.Columns.Add(columnDbName);
		var updateResult = _spaceManager.UpdateSpaceData(spaceData.ToModel());
		if (updateResult.IsFailed) return Result.Fail(new Error("UpdateSpaceData failed").CausedBy(updateResult.Errors));

		return Result.Ok(new TucSpaceData(updateResult.Value));

	}

	internal Result<TucSpaceData> RemoveColumnFromSpaceData(Guid spaceDataId, string columnDbName)
	{
		if (spaceDataId == Guid.Empty) return Result.Fail("spaceDataId is required");
		if (String.IsNullOrWhiteSpace(columnDbName)) return Result.Fail("columnDbName is required");
		var spaceData = GetSpaceData(spaceDataId);
		if (spaceData is null) return Result.Fail("spaceData not found");
		if (!spaceData.Columns.Contains(columnDbName)) return Result.Ok(spaceData);

		spaceData.Columns.Remove(columnDbName);
		var updateResult = _spaceManager.UpdateSpaceData(spaceData.ToModel());
		if (updateResult.IsFailed) return Result.Fail(new Error("UpdateSpaceData failed").CausedBy(updateResult.Errors));

		return Result.Ok(new TucSpaceData(updateResult.Value));

	}

	internal Result<TucSpaceData> UpdateSpaceDataFilters(Guid spaceDataId, List<TucFilterBase> filters)
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

	internal Result<TucSpaceData> AddSortColumnToSpaceData(Guid spaceDataId, TucSort sort)
	{
		if (spaceDataId == Guid.Empty) return Result.Fail("spaceDataId is required");
		if (sort is null || String.IsNullOrWhiteSpace(sort.DbName)) return Result.Fail("sort is required");
		var spaceData = GetSpaceData(spaceDataId);
		if (spaceData is null) return Result.Fail("spaceData not found");
		if (spaceData.SortOrders.Any(x => x.DbName == sort.DbName)) return Result.Ok(spaceData);

		spaceData.SortOrders.Add(sort);
		var updateResult = _spaceManager.UpdateSpaceData(spaceData.ToModel());
		if (updateResult.IsFailed) return Result.Fail(new Error("UpdateSpaceData failed").CausedBy(updateResult.Errors));

		return Result.Ok(new TucSpaceData(updateResult.Value));

	}

	internal Result<TucSpaceData> RemoveSortColumnFromSpaceData(Guid spaceDataId, TucSort sort)
	{
		if (spaceDataId == Guid.Empty) return Result.Fail("spaceDataId is required");
		if (sort is null || String.IsNullOrWhiteSpace(sort.DbName)) return Result.Fail("sort is required");
		var spaceData = GetSpaceData(spaceDataId);
		if (spaceData is null) return Result.Fail("spaceData not found");
		if (!spaceData.SortOrders.Any(x => x.DbName == sort.DbName)) return Result.Ok(spaceData);

		spaceData.SortOrders = spaceData.SortOrders.Where(x => x.DbName != sort.DbName).ToList();
		var updateResult = _spaceManager.UpdateSpaceData(spaceData.ToModel());
		if (updateResult.IsFailed) return Result.Fail(new Error("UpdateSpaceData failed").CausedBy(updateResult.Errors));

		return Result.Ok(new TucSpaceData(updateResult.Value));

	}

	//Data provider
	internal List<TucDataProvider> GetDataProviderList()
	{
		var serviceResult = _dataProviderManager.GetProviders();
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetProviders failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
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
