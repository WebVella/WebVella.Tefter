namespace WebVella.Tefter.UseCases.AppState;
internal partial class AppStateUseCase
{
	internal async Task<(TfAppState, TfAuxDataState)> InitAdminDataProviderAsync(
		IServiceProvider serviceProvider,
		TucUser currentUser,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
	{
		if (
			!(newAppState.Route.FirstNode == RouteDataFirstNode.Admin
			&& newAppState.Route.SecondNode == RouteDataSecondNode.DataProviders)
			)
		{
			newAppState = newAppState with
			{
				AdminDataProviders = new(),
				AdminDataProvider = null,
				DataProviderTypes = new(),
				DataProviderSyncTasks = new(),
				AdminDataProviderData = null,
			};
			return (newAppState, newAuxDataState);
		};


		//AdminDataProviders, AdminDataProvidersPage
		if (
			newAppState.AdminDataProviders.Count == 0
			|| (newAppState.Route.DataProviderId is not null && !newAppState.AdminDataProviders.Any(x => x.Id == newAppState.Route.DataProviderId))
			)
			newAppState = newAppState with
			{
				AdminDataProviders = await GetDataProvidersAsync()
			};

		newAppState = newAppState with { DataProviderTypes = await GetProviderTypesAsync() };

		//AdminManagedUser, DataProviderTypes, DataProviderSyncTasks, DataProviderSyncTasks, 
		if (newAppState.Route.DataProviderId.HasValue)
		{
			var adminProvider = await GetDataProviderAsync(newAppState.Route.DataProviderId.Value);
			newAppState = newAppState with { AdminDataProvider = adminProvider };
			if (adminProvider is not null)
			{
				if (!newAppState.AdminDataProviders.Any(x => x.Id == adminProvider.Id))
				{
					var adminProviders = newAppState.AdminDataProviders.ToList();
					adminProviders.Add(adminProvider);
					newAppState = newAppState with { AdminDataProviders = adminProviders };
				}

				//check for the other tabs
				if (newAppState.Route.ThirdNode == RouteDataThirdNode.Schema)
				{
				}
				else if (newAppState.Route.ThirdNode == RouteDataThirdNode.SharedKeys)
				{
				}
			}

			if (newAppState.Route.ThirdNode == RouteDataThirdNode.Synchronization)
			{
				newAppState = newAppState with { DataProviderSyncTasks = await GetDataProviderSynchronizationTasks(newAppState.Route.DataProviderId.Value) };
			}
			else
			{
				newAppState = newAppState with { DataProviderSyncTasks = new() };
			}
			if (newAppState.Route.ThirdNode == RouteDataThirdNode.Data)
			{
				var dt = await GetDataProviderData(newAppState.Route.DataProviderId.Value, newAppState.Route.Search,
					(newAppState.Route.Page ?? 1), (newAppState.Route.PageSize ?? TfConstants.PageSize));
				newAppState = newAppState with
				{
					AdminDataProviderData = dt
				};
			}
			else
			{
				newAppState = newAppState with { AdminDataProviderData = null };
			}
		}


		return (newAppState, newAuxDataState);
	}

	//Data Provider
	internal virtual Task<List<TucDataProvider>> GetDataProvidersAsync(string search = null, int? page = null, int? pageSize = null)
	{
		var srvResult = _dataProviderManager.GetProviders();
		if (srvResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetProviders failed").CausedBy(srvResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.FromResult(new List<TucDataProvider>());
		}

		if (srvResult.Value is null) return Task.FromResult(new List<TucDataProvider>());

		var orderedResults = srvResult.Value.OrderBy(x => x.Name);

		var records = new List<TfDataProvider>();
		if (!String.IsNullOrWhiteSpace(search))
		{
			var searchProcessed = search.Trim().ToLowerInvariant();
			foreach (var item in orderedResults)
			{
				bool hasMatch = false;
				if (item.Name.ToLowerInvariant().Contains(searchProcessed)) hasMatch = true;
				if (hasMatch) records.Add(item);
			}
		}
		else records = orderedResults.ToList();

		if (page is null || pageSize is null) return Task.FromResult(records.Select(x => new TucDataProvider(x)).ToList());

		return Task.FromResult(records.Skip(TfConverters.CalcSkip(page.Value, pageSize.Value)).Take(pageSize.Value).Select(x => new TucDataProvider(x)).ToList());
	}
	internal virtual Task<TucDataProvider> GetDataProviderAsync(Guid providerId)
	{
		var srvResult = _dataProviderManager.GetProvider(providerId);
		if (srvResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetProvider failed").CausedBy(srvResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.FromResult(new TucDataProvider());
		}
		if (srvResult.Value is null) return Task.FromResult((TucDataProvider)null);


		return Task.FromResult(new TucDataProvider(srvResult.Value));
	}

	internal virtual Task<List<TucDataProviderTypeInfo>> GetProviderTypesAsync()
	{
		var srvResult = _dataProviderManager.GetProviderTypes();
		if (srvResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetProviderTypes failed").CausedBy(srvResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.FromResult(new List<TucDataProviderTypeInfo>());
		}
		if (srvResult.Value is null) return Task.FromResult(new List<TucDataProviderTypeInfo>());

		return Task.FromResult(srvResult.Value.Select(t => new TucDataProviderTypeInfo(t)).ToList());
	}
	internal virtual Task<Result> DeleteDataProviderAsync(Guid providerId)
	{
		var srvResult = _dataProviderManager.DeleteDataProvider(providerId);
		if (srvResult.IsFailed)
		{
			return Task.FromResult(Result.Fail(new Error("DeleteDataProvider failed").CausedBy(srvResult.Errors)));
		}

		return Task.FromResult(Result.Ok());
	}

	internal virtual Result<TucDataProvider> CreateDataProviderWithForm(TucDataProviderForm form)
	{
		var providerTypesResult = _dataProviderManager.GetProviderTypes();
		if (providerTypesResult.IsFailed) return Result.Fail(new Error("GetProviderTypes failed").CausedBy(providerTypesResult.Errors));
		var submitForm = form.ToModel(providerTypesResult.Value);
		var createResult = _dataProviderManager.CreateDataProvider(submitForm);
		if (createResult.IsFailed) return Result.Fail(new Error("CreateDataProvider failed").CausedBy(createResult.Errors));
		if (createResult.Value is null) return Result.Fail(new Error("CreateDataProvider returned null object").CausedBy(createResult.Errors));
		return Result.Ok(new TucDataProvider(createResult.Value));
	}

	internal virtual Result<TucDataProvider> UpdateDataProviderWithForm(TucDataProviderForm form)
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
	internal virtual Result<TucDataProvider> CreateDataProviderColumn(TucDataProviderColumnForm form)
	{
		var result = _dataProviderManager.CreateDataProviderColumn(form.ToModel());
		if (result.IsFailed)
			return Result.Fail(new Error("CreateDataProviderColumn failed").CausedBy(result.Errors));

		return Result.Ok(new TucDataProvider(result.Value));
	}
	internal virtual Result<TucDataProvider> UpdateDataProviderColumn(TucDataProviderColumnForm form)
	{
		var result = _dataProviderManager.UpdateDataProviderColumn(form.ToModel());
		if (result.IsFailed)
			return Result.Fail(new Error("UpdateDataProviderColumn failed").CausedBy(result.Errors));

		return Result.Ok(new TucDataProvider(result.Value));
	}
	internal virtual Result<TucDataProvider> DeleteDataProviderColumn(Guid columnId)
	{
		var result = _dataProviderManager.DeleteDataProviderColumn(columnId);
		if (result.IsFailed)
			return Result.Fail(new Error("DeleteDataProviderColumn failed").CausedBy(result.Errors));

		return Result.Ok(new TucDataProvider(result.Value));
	}

	//Data provider key

	internal virtual Result<TucDataProvider> CreateDataProviderKey(TucDataProviderSharedKeyForm form)
	{
		var result = _dataProviderManager.CreateDataProviderSharedKey(form.ToModel());
		if (result.IsFailed)
			return Result.Fail(new Error("CreateDataProviderSharedKey failed").CausedBy(result.Errors));

		return Result.Ok(new TucDataProvider(result.Value));
	}

	internal virtual Result<TucDataProvider> UpdateDataProviderKey(TucDataProviderSharedKeyForm form)
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
	internal virtual Task<List<TucDataProviderSyncTask>> GetDataProviderSynchronizationTasks(Guid providerId)
	{
		var srvResult = _dataProviderManager.GetSynchronizationTasksExtended(providerId);
		if (srvResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSynchronizationTasksExtended failed").CausedBy(srvResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
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
	internal virtual Task TriggerSynchronization(Guid dataProviderId)
	{
		var createResult = _dataProviderManager.CreateSynchronizationTask(dataProviderId, new TfSynchronizationPolicy());
		if (createResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("CreateSynchronizationTask failed").CausedBy(createResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.CompletedTask;
		}
		return Task.CompletedTask;
	}

	internal virtual Task DeleteAllProviderData(Guid dataProviderId)
	{
		var provider = _dataProviderManager.GetProvider(dataProviderId);
		if (provider is null)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail("Provider not found"),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.CompletedTask;
		}
		var createResult = _dataManager.DeleteAllProviderRows(provider.Value);
		if (createResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("DeleteAllProviderRows failed").CausedBy(createResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.CompletedTask;
		}
		return Task.CompletedTask;
	}

	internal virtual Result<List<TucDataProviderSyncTaskInfo>> GetSynchronizationTaskLogRecords(Guid taskId,
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
	internal virtual Result<TfDataTable> GetDataProviderDataResult(Guid providerId, string search = null, int? page = null, int? pageSize = null)
	{
		var srvProviderResult = _dataProviderManager.GetProvider(providerId);
		if (srvProviderResult.IsFailed)
			return Result.Fail(new Error("GetProvider failed").CausedBy(srvProviderResult.Errors));

		if (srvProviderResult.Value is null) return Result.Fail("Provider not found");

		var dtResult = _dataManager.QueryDataProvider(
			provider: srvProviderResult.Value,
			search: search,
			page: page,
			pageSize: pageSize);

		if (dtResult.IsFailed)
			return Result.Fail(new Error("QueryDataProvider failed").CausedBy(dtResult.Errors));

		return Result.Ok(dtResult.Value);
	}
	internal virtual Task<TfDataTable> GetDataProviderData(Guid providerId, string search = null, int? page = null, int? pageSize = null)
	{
		var srvProviderResult = _dataProviderManager.GetProvider(providerId);
		if (srvProviderResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetProvider failed").CausedBy(srvProviderResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
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
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.FromResult((TfDataTable)null);
		}
		return Task.FromResult(dtResult.Value);
	}
}
