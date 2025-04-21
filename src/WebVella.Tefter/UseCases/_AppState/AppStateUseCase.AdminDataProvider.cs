using Microsoft.AspNetCore.Mvc;

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
		}
		;


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
				else if (newAppState.Route.ThirdNode == RouteDataThirdNode.JoinKeys)
				{
				}
			}

			if (newAppState.Route.ThirdNode == RouteDataThirdNode.Synchronization)
			{
				var tasks = await GetDataProviderSynchronizationTasks(newAppState.Route.DataProviderId.Value);
				var pageSize = 5;
				var page = newAppState.Route.Page ?? 1;
				tasks = tasks.Skip(TfConverters.CalcSkip(page, pageSize)).Take(pageSize).ToList();
				newAppState = newAppState with { DataProviderSyncTasks = tasks };
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
					AdminDataProviderData = dt,
					Route = newAppState.Route with { Page = dt.QueryInfo.Page }
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
	internal virtual Task<List<TucDataProvider>> GetDataProvidersAsync(
		string search = null,
		int? page = null,
		int? pageSize = null)
	{
		try
		{
			var providers = _tfService.GetDataProviders();
			if (providers is null)
				return Task.FromResult(new List<TucDataProvider>());

			var orderedResults = providers.OrderBy(x => x.Name);

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
			else
			{
				records = orderedResults.ToList();
			}

			if (page is null || pageSize is null)
			{
				return Task.FromResult(records
					.Select(x => new TucDataProvider(x))
					.ToList());
			}

			return Task.FromResult(records
					.Skip(TfConverters.CalcSkip(page.Value, pageSize.Value))
					.Take(pageSize.Value)
					.Select(x => new TucDataProvider(x))
					.ToList());
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
			return Task.FromResult(new List<TucDataProvider>());
		}
	}

	internal virtual Task<TucDataProvider> GetDataProviderAsync(
		Guid providerId)
	{
		try
		{
			var provider = _tfService.GetDataProvider(providerId);
			if (provider is null)
				return Task.FromResult((TucDataProvider)null);

			return Task.FromResult(new TucDataProvider(provider));
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
			return Task.FromResult(new TucDataProvider());

		}
	}

	internal virtual Task<long> GetDataProviderTotalRowCountAsync(Guid providerId)
	{
		try
		{
			return Task.FromResult(_tfService.GetDataProviderRowsCount(providerId));
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
			return Task.FromResult((long)0);

		}
	}

	internal virtual Task<List<TucDataProvider>> GetDataProviderJoinedProvidersAsync(Guid providerId)
	{
		var result = new List<TucDataProvider>();
		try
		{
			var providers = _tfService.GetAvailableForJoinDataProviders(providerId);
			if (providers is not null)
			{
				foreach (var item in providers)
					result.Add(new TucDataProvider(item));
			}
			return Task.FromResult(result);
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
			return Task.FromResult(result);

		}
	}

	internal virtual Task<ReadOnlyCollection<TucDataProviderInfo>> GetDataProvidersInfoAsync()
	{
		try
		{
			var providersInfo = _tfService.GetDataProvidersInfo();
			if (providersInfo is null)
				return Task.FromResult((new List<TucDataProviderInfo>()).AsReadOnly());

			var result = new List<TucDataProviderInfo>();
			foreach (var item in providersInfo)
			{
				result.Add(new TucDataProviderInfo(item));
			}

			return Task.FromResult(result.AsReadOnly());
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
			return Task.FromResult((new List<TucDataProviderInfo>()).AsReadOnly());

		}
	}

	internal virtual Task<List<TucDataProviderTypeInfo>> GetProviderTypesAsync()
	{
		try
		{
			var providerTypes = _metaService.GetDataProviderTypes();
			if (providerTypes is null)
				return Task.FromResult(new List<TucDataProviderTypeInfo>());

			return Task.FromResult(providerTypes.Select(t => new TucDataProviderTypeInfo(t)).ToList());
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
			return Task.FromResult(new List<TucDataProviderTypeInfo>());
		}
	}

	internal virtual Task DeleteDataProviderAsync(
		Guid providerId)
	{
		_tfService.DeleteDataProvider(providerId);
		return Task.CompletedTask;
	}

	internal virtual TucDataProvider CreateDataProviderWithForm(
		TucDataProviderForm form)
	{
		var providerTypes = _metaService.GetDataProviderTypes();

		var submitForm = form.ToModel(providerTypes, null);

		var provider = _tfService.CreateDataProvider(submitForm);
		if (provider is null)
			throw new TfException("CreateDataProvider returned null object");

		return new TucDataProvider(provider);
	}

	internal virtual TucDataProvider UpdateDataProviderWithForm(
		TucDataProviderForm form)
	{
		var providerTypes = _metaService.GetDataProviderTypes();
		var providerSM = _tfService.GetDataProvider(form.Id);
		if (providerSM is null)
			throw new TfException("CreateDataProvider provider not found");

		var submitForm = form.ToModel(providerTypes, providerSM);

		var provider = _tfService.UpdateDataProvider(submitForm);
		if (provider is null)
			throw new TfException("UpdateDataProvider returned null object");

		return new TucDataProvider(provider);
	}

	internal virtual TucDataProvider UpdateDataProviderSynchPrimaryKeyColumns(
		Guid providerId,
		List<string> columns)
	{
		var provider = _tfService.GetDataProvider(providerId);
		if (provider is null)
			throw new TfException("Provider not found");

		var submit = new TfDataProviderModel
		{
			Id = providerId,
			SynchPrimaryKeyColumns = columns,
			Name = provider.Name,
			ProviderType = provider.ProviderType,
			SettingsJson = provider.SettingsJson,
			SynchScheduleEnabled = provider.SynchScheduleEnabled,
			SynchScheduleMinutes = provider.SynchScheduleMinutes
		};

		provider = _tfService.UpdateDataProvider(submit);
		if (provider is null)
			throw new TfException("UpdateDataProvider returned null object");

		return new TucDataProvider(provider);
	}

	internal virtual TucDataProvider UpdateDataProviderSunchronization(
		TucDataProviderUpdateSyncForm form)
	{
		var providerTypes = _metaService.GetDataProviderTypes();
		var providerSM = _tfService.GetDataProvider(form.Id);
		if (providerSM is null)
			throw new TfException("CreateDataProvider provider not found");

		var submitForm = form.ToModel(providerSM);

		var provider = _tfService.UpdateDataProvider(submitForm);
		if (provider is null)
			throw new TfException("UpdateDataProvider returned null object");

		return new TucDataProvider(provider);
	}

	//Data provider columns
	internal virtual TucDataProvider CreateDataProviderColumn(
		TucDataProviderColumnForm form)
	{
		var result = _tfService.CreateDataProviderColumn(form.ToModel());
		return new TucDataProvider(result);
	}

	internal virtual TucDataProvider UpdateDataProviderColumn(
		TucDataProviderColumnForm form)
	{
		var result = _tfService.UpdateDataProviderColumn(form.ToModel());
		return new TucDataProvider(result);
	}

	internal virtual TucDataProvider DeleteDataProviderColumn(
		Guid columnId)
	{
		var result = _tfService.DeleteDataProviderColumn(columnId);
		return new TucDataProvider(result);
	}

	internal virtual TucDataProvider CreateBulkDataProviderColumn(
		Guid providerId,
		List<TucDataProviderColumn> columns)
	{
		var columnSM = columns.Select(x => x.ToModel()).ToList();
		var result = _tfService.CreateBulkDataProviderColumn(providerId, columnSM);
		return new TucDataProvider(result);
	}

	internal virtual Task<TucDataProviderSourceSchemaInfo> GetDataProviderSourceSchemaInfo(TucDataProvider provider)
	{
		try
		{
			var result = _tfService.GetDataProviderSourceSchemaInfo(provider.Id);
			return Task.FromResult(new TucDataProviderSourceSchemaInfo(result));
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
			return Task.FromResult(new TucDataProviderSourceSchemaInfo());
		}
	}

	//Data provider key
	internal virtual TucDataProvider CreateDataProviderKey(
		TucDataProviderJoinKeyForm form)
	{
		var result = _tfService.CreateDataProviderJoinKey(form.ToModel());
		return new TucDataProvider(result);
	}

	internal virtual TucDataProvider UpdateDataProviderKey(
		TucDataProviderJoinKeyForm form)
	{
		var result = _tfService.UpdateDataProviderJoinKey(form.ToModel());
		return new TucDataProvider(result);
	}

	internal TucDataProvider DeleteDataProviderJoinKey(
		Guid keyId)
	{
		var result = _tfService.DeleteDataProviderJoinKey(keyId);
		return new TucDataProvider(result);
	}

	//Data synchronization
	internal virtual Task<List<TucDataProviderSyncTask>> GetDataProviderSynchronizationTasks(
		Guid providerId)
	{
		try
		{
			var result = _tfService.GetSynchronizationTasks(providerId);

			if (result is null)
				return Task.FromResult(new List<TucDataProviderSyncTask>());

			var tasks = result
				.OrderByDescending(x => x.CreatedOn)
				.Take(TfConstants.PageSize)
				.Select(x => new TucDataProviderSyncTask(x))
				.ToList();

			return Task.FromResult(tasks);
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

			return Task.FromResult(new List<TucDataProviderSyncTask>());
		}
	}

	internal virtual Task TriggerSynchronization(
		Guid dataProviderId)
	{
		try
		{
			_tfService.CreateSynchronizationTask(dataProviderId, new TfSynchronizationPolicy());
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
		return Task.CompletedTask;
	}

	internal virtual DateTime? GetProviderNextSyncOn(
			Guid dataProviderId)
	{
		try
		{
			return _tfService.GetDataProviderNextSynchronizationTime(dataProviderId);
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
		return null;
	}

	internal virtual Task DeleteAllProviderData(
		Guid dataProviderId)
	{
		var provider = _tfService.GetDataProvider(dataProviderId);
		if (provider is null)
		{
			ResultUtils.ProcessServiceException(
				exception: new TfException("Provider not found"),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.CompletedTask;
		}
		_tfService.DeleteAllProviderRows(provider);
		return Task.CompletedTask;
	}

	//internal virtual List<TucDataProviderSyncTaskInfo> GetSynchronizationTaskLogRecords(
	//	Guid taskId,
	//	TucDataProviderSyncTaskInfoType type)
	//{
	//	var allTasks = _tfService.GetSynchronizationTaskResultInfos(taskId);
	//	var result = new List<TucDataProviderSyncTaskInfo>();
	//	switch (type)
	//	{
	//		case TucDataProviderSyncTaskInfoType.Info:
	//			result = allTasks.Where(x => !String.IsNullOrWhiteSpace(x.Info))
	//			.Select(x => new TucDataProviderSyncTaskInfo(x)).ToList();
	//			break;
	//		case TucDataProviderSyncTaskInfoType.Warning:
	//			result = allTasks.Where(x => !String.IsNullOrWhiteSpace(x.Warning))
	//			.Select(x => new TucDataProviderSyncTaskInfo(x)).ToList();
	//			break;
	//		case TucDataProviderSyncTaskInfoType.Error:
	//			result = allTasks.Where(x => !String.IsNullOrWhiteSpace(x.Error))
	//			.Select(x => new TucDataProviderSyncTaskInfo(x)).ToList();
	//			break;
	//		default:
	//			throw new TfException("Not supported TucDataProviderSyncTaskInfoType");
	//	}
	//	return result;
	//}

	//Data
	internal virtual TfDataTable GetDataProviderDataResult(Guid providerId, string search = null, int? page = null, int? pageSize = null)
	{
		var provider = _tfService.GetDataProvider(providerId);
		if (provider is null)
			throw new TfException("Provider not found");

		var dt = _tfService.QueryDataProvider(
			provider: provider,
			search: search,
			page: page,
			pageSize: pageSize);

		return dt;
	}

	internal virtual Task<TfDataTable> GetDataProviderData(Guid providerId, string search = null, int? page = null, int? pageSize = null)
	{
		try
		{
			var provider = _tfService.GetDataProvider(providerId);

			if (provider is null) return Task.FromResult((TfDataTable)null);

			var data = _tfService.QueryDataProvider(
				provider: provider,
				search: search,
				page: page,
				pageSize: pageSize);

			return Task.FromResult(data);
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
			return Task.FromResult((TfDataTable)null);
		}
	}

}
