namespace WebVella.Tefter.UseCases.AppStart;
internal partial class AppStateUseCase
{
	internal async Task<TfAppState> InitAdminDataProvider(TucUser currentUser, TfRouteState routeState, TfAppState result)
	{
		if (
			!(routeState.FirstNode == RouteDataFirstNode.Admin
			&& routeState.SecondNode == RouteDataSecondNode.DataProviders)
			)
		{
			result = result with
			{
				AdminDataProviders = new(),
				AdminDataProvidersPage = 0,
				AdminDataProvider = null,
				DataProviderTypes = new(),
				DataProviderSyncTasks = new(),
				AdminDataProviderData = null,
				AdminDataProviderDataPage = 0
			};
			return result;
		};


		//AdminDataProviders, AdminDataProvidersPage
		if (result.AdminDataProviders.Count == 0)
			result = result with
			{
				AdminDataProviders = await GetDataProvidersAsync(null, 1, TfConstants.PageSize),
				AdminDataProvidersPage = 1
			};

		//AdminManagedUser, DataProviderTypes, DataProviderSyncTasks, DataProviderSyncTasks, 
		if (routeState.DataProviderId.HasValue)
		{
			var adminProvider = await GetDataProviderAsync(routeState.DataProviderId.Value);
			result = result with { AdminDataProvider = adminProvider };
			if (adminProvider is not null)
			{
				if (!result.AdminDataProviders.Any(x => x.Id == adminProvider.Id))
				{
					var adminProviders = result.AdminDataProviders.ToList();
					adminProviders.Add(adminProvider);
					result = result with { AdminDataProviders = adminProviders };
				}

				//check for the other tabs
				if (routeState.ThirdNode == RouteDataThirdNode.Schema)
				{
				}
				else if (routeState.ThirdNode == RouteDataThirdNode.SharedKeys)
				{
				}
			}

			result = result with { DataProviderTypes = await GetProviderTypesAsync() };

			if (routeState.ThirdNode == RouteDataThirdNode.Synchronization)
			{
				result = result with { DataProviderSyncTasks = await GetDataProviderSynchronizationTasks(routeState.DataProviderId.Value) };
			}
			else
			{
				result = result with { DataProviderSyncTasks = new() };
			}
			if (routeState.ThirdNode == RouteDataThirdNode.Data)
			{
				result = result with
				{
					AdminDataProviderData = await GetDataProviderData(routeState.DataProviderId.Value, null, 1, TfConstants.PageSize),
					AdminDataProviderDataPage = 1
				};
			}
			else
			{
				result = result with { AdminDataProviderData = null, AdminDataProviderDataPage = 0 };
			}
		}


		return result;
	}

	//Data Provider
	internal Task<List<TucDataProvider>> GetDataProvidersAsync(string search = null, int? page = null, int? pageSize = null)
	{
		var srvResult = _dataProviderManager.GetProviders();
		if (srvResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetProviders failed").CausedBy(srvResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.FromResult(new List<TucDataProvider>());
		}

		if (srvResult.Value is null) return Task.FromResult(new List<TucDataProvider>());

		var records = new List<TfDataProvider>();
		if (!String.IsNullOrWhiteSpace(search))
		{
			var searchProcessed = search.Trim().ToLowerInvariant();
			foreach (var item in srvResult.Value)
			{
				bool hasMatch = false;
				if (item.Name.ToLowerInvariant().Contains(searchProcessed)) hasMatch = true;
				if (hasMatch) records.Add(item);
			}
		}
		else records = srvResult.Value.ToList();

		if (page is null || pageSize is null) return Task.FromResult(records.Select(x => new TucDataProvider(x)).ToList());

		return Task.FromResult(records.Skip(RenderUtils.CalcSkip(page.Value, pageSize.Value)).Take(pageSize.Value).Select(x => new TucDataProvider(x)).ToList());
	}
	internal Task<TucDataProvider> GetDataProviderAsync(Guid providerId)
	{
		var srvResult = _dataProviderManager.GetProvider(providerId);
		if (srvResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetProvider failed").CausedBy(srvResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.FromResult(new TucDataProvider());
		}
		if (srvResult.Value is null) return Task.FromResult((TucDataProvider)null);


		return Task.FromResult(new TucDataProvider(srvResult.Value));
	}

	internal Task<List<TucDataProviderTypeInfo>> GetProviderTypesAsync()
	{
		var srvResult = _dataProviderManager.GetProviderTypes();
		if (srvResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetProviderTypes failed").CausedBy(srvResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.FromResult(new List<TucDataProviderTypeInfo>());
		}
		if (srvResult.Value is null) return Task.FromResult(new List<TucDataProviderTypeInfo>());

		return Task.FromResult(srvResult.Value.Select(t => new TucDataProviderTypeInfo(t)).ToList());
	}
	internal Task<Result> DeleteDataProviderAsync(Guid providerId)
	{
		var srvResult = _dataProviderManager.DeleteDataProvider(providerId);
		if (srvResult.IsFailed)
		{
			return Task.FromResult(Result.Fail(new Error("DeleteDataProvider failed").CausedBy(srvResult.Errors)));
		}

		return Task.FromResult(Result.Ok());
	}

	internal Result<TucDataProvider> CreateDataProviderWithForm(TucDataProviderForm form)
	{
		var providerTypesResult = _dataProviderManager.GetProviderTypes();
		if (providerTypesResult.IsFailed) return Result.Fail(new Error("GetProviderTypes failed").CausedBy(providerTypesResult.Errors));
		var submitForm = form.ToModel(providerTypesResult.Value);
		var createResult = _dataProviderManager.CreateDataProvider(submitForm);
		if (createResult.IsFailed) return Result.Fail(new Error("CreateDataProvider failed").CausedBy(createResult.Errors));
		if (createResult.Value is null) return Result.Fail(new Error("CreateDataProvider returned null object").CausedBy(createResult.Errors));
		return Result.Ok(new TucDataProvider(createResult.Value));
	}

	internal Result<TucDataProvider> UpdateDataProviderWithForm(TucDataProviderForm form)
	{
		var providerTypesResult = _dataProviderManager.GetProviderTypes();
		if (providerTypesResult.IsFailed) return Result.Fail(new Error("GetProviderTypes failed").CausedBy(providerTypesResult.Errors));
		var submitForm = form.ToModel(providerTypesResult.Value);
		var updateResult = _dataProviderManager.UpdateDataProvider(submitForm);
		if (updateResult.IsFailed) return Result.Fail(new Error("UpdateDataProvider failed").CausedBy(updateResult.Errors));
		if (updateResult.Value is null) return Result.Fail(new Error("UpdateDataProvider returned null object").CausedBy(updateResult.Errors));
		return Result.Ok(new TucDataProvider(updateResult.Value));
	}

	//Data provider columns
	internal Result<TucDataProvider> CreateDataProviderColumn(TucDataProviderColumnForm form)
	{
		var result = _dataProviderManager.CreateDataProviderColumn(form.ToModel());
		if (result.IsFailed)
			return Result.Fail(new Error("CreateDataProviderColumn failed").CausedBy(result.Errors));

		return Result.Ok(new TucDataProvider(result.Value));
	}
	internal Result<TucDataProvider> UpdateDataProviderColumn(TucDataProviderColumnForm form)
	{
		var result = _dataProviderManager.UpdateDataProviderColumn(form.ToModel());
		if (result.IsFailed)
			return Result.Fail(new Error("UpdateDataProviderColumn failed").CausedBy(result.Errors));

		return Result.Ok(new TucDataProvider(result.Value));
	}
	internal Result<TucDataProvider> DeleteDataProviderColumn(Guid columnId)
	{
		var result = _dataProviderManager.DeleteDataProviderColumn(columnId);
		if (result.IsFailed)
			return Result.Fail(new Error("DeleteDataProviderColumn failed").CausedBy(result.Errors));

		return Result.Ok(new TucDataProvider(result.Value));
	}

	//Data provider key

	internal Result<TucDataProvider> CreateDataProviderKey(TucDataProviderSharedKeyForm form)
	{
		var result = _dataProviderManager.CreateDataProviderSharedKey(form.ToModel());
		if (result.IsFailed)
			return Result.Fail(new Error("CreateDataProviderSharedKey failed").CausedBy(result.Errors));

		return Result.Ok(new TucDataProvider(result.Value));
	}

	internal Result<TucDataProvider> UpdateDataProviderKey(TucDataProviderSharedKeyForm form)
	{
		var result = _dataProviderManager.UpdateDataProviderSharedKey(form.ToModel());
		if (result.IsFailed)
			return Result.Fail(new Error("UpdateDataProviderColumn failed").CausedBy(result.Errors));

		return Result.Ok(new TucDataProvider(result.Value));
	}
	internal Result<TucDataProvider> DeleteDataProviderSharedKey(Guid keyId)
	{
		var result = _dataProviderManager.DeleteDataProviderSharedKey(keyId);
		if (result.IsFailed)
			return Result.Fail(new Error("DeleteDataProviderSharedKey failed").CausedBy(result.Errors));

		return Result.Ok(new TucDataProvider(result.Value));
	}

	//Data synchronization
	internal Task<List<TucDataProviderSyncTask>> GetDataProviderSynchronizationTasks(Guid providerId)
	{
		var srvResult = _dataProviderManager.GetSynchronizationTasksExtended(providerId);
		if (srvResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSynchronizationTasksExtended failed").CausedBy(srvResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.FromResult(new List<TucDataProviderSyncTask>());
		}
		if (srvResult.Value is null) return Task.FromResult(new List<TucDataProviderSyncTask>());

		var tasks = srvResult.Value.OrderByDescending(x => x.CreatedOn).Take(TfConstants.PageSize).Select(x => new TucDataProviderSyncTask(x)).ToList();
		return Task.FromResult(tasks);
	}
	internal Task TriggerSynchronization(Guid dataProviderId)
	{
		var createResult = _dataProviderManager.CreateSynchronizationTask(dataProviderId, new TfSynchronizationPolicy());
		if (createResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("CreateSynchronizationTask failed").CausedBy(createResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.CompletedTask;
		}
		return Task.CompletedTask;
	}

	internal Result<List<TucDataProviderSyncTaskInfo>> GetSynchronizationTaskLogRecords(Guid taskId,
			TucDataProviderSyncTaskInfoType type)
	{
		var allTasksResult = _dataProviderManager.GetSynchronizationTaskResultInfos(taskId);
		if (allTasksResult.IsFailed)
			return Result.Fail(new Error("GetSynchronizationTaskResultInfos failed").CausedBy(allTasksResult.Errors));
		var result = new List<TucDataProviderSyncTaskInfo>();
		switch (type)
		{
			case TucDataProviderSyncTaskInfoType.Info:
				result = allTasksResult.Value.Where(x => !String.IsNullOrWhiteSpace(x.Info))
				.Select(x => new TucDataProviderSyncTaskInfo(x)).ToList();
				break;
			case TucDataProviderSyncTaskInfoType.Warning:
				result = allTasksResult.Value.Where(x => !String.IsNullOrWhiteSpace(x.Warning))
				.Select(x => new TucDataProviderSyncTaskInfo(x)).ToList();
				break;
			case TucDataProviderSyncTaskInfoType.Error:
				result = allTasksResult.Value.Where(x => !String.IsNullOrWhiteSpace(x.Error))
				.Select(x => new TucDataProviderSyncTaskInfo(x)).ToList();
				break;
			default:
				return Result.Fail("Not supported TucDataProviderSyncTaskInfoType");
		}
		return Result.Ok(result);
	}

	//Data
	internal Result<TfDataTable> GetDataProviderDataResult(Guid providerId, string search = null, int? page = null, int? pageSize = null)
	{
		var srvProviderResult = _dataProviderManager.GetProvider(providerId);
		if (srvProviderResult.IsFailed)
			return Result.Fail(new Error("DeleteDataProviderSharedKey failed").CausedBy(srvProviderResult.Errors));

		if (srvProviderResult.Value is null) return Result.Fail("Provider not found");

		var dtResult = _dataManager.QueryDataProvider(
			provider: srvProviderResult.Value,
			search: search,
			page: page,
			pageSize: pageSize);

		if (dtResult.IsFailed)
			return Result.Fail(new Error("DeleteDataProviderSharedKey failed").CausedBy(dtResult.Errors));

		return Result.Ok(dtResult.Value);
	}
	internal Task<TfDataTable> GetDataProviderData(Guid providerId, string search = null, int? page = null, int? pageSize = null)
	{
		var srvProviderResult = _dataProviderManager.GetProvider(providerId);
		if (srvProviderResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetProvider failed").CausedBy(srvProviderResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.FromResult((TfDataTable)null);
		}

		if (srvProviderResult.Value is null) return Task.FromResult((TfDataTable)null);

		var dtResult = _dataManager.QueryDataProvider(
			provider: srvProviderResult.Value,
			search: search,
			page: page,
			pageSize: pageSize);

		if (dtResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("QueryDataProvider failed").CausedBy(srvProviderResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.FromResult((TfDataTable)null);
		}
		return Task.FromResult(dtResult.Value);
	}
}
