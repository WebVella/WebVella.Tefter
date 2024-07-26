namespace WebVella.Tefter.UseCases.DataProviderAdmin;
public partial class DataProviderAdminUseCase
{
	internal bool IsSynchronizing { get; set; } = false;
	internal IQueryable<TucDataProviderSyncTask> SyncTasks { get; set; } = Enumerable.Empty<TucDataProviderSyncTask>().AsQueryable();
	internal int SyncLogPageSize { get; set; } = 15;


	internal Task InitForSynchronization()
	{
		IsBusy = true;
		return Task.CompletedTask;
	}

	internal Task LoadDataProviderDataObjects(Guid providerId)
	{
		var allTasksResult = _dataProviderManager.GetSynchronizationTasksExtended(providerId);
		if (allTasksResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSynchronizationTasksExtended failed").CausedBy(allTasksResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.CompletedTask;
		}
		var tasks = allTasksResult.Value.OrderByDescending(x => x.CreatedOn);
		SyncTasks = tasks.Select(x => new TucDataProviderSyncTask(x)).AsQueryable();
		return Task.CompletedTask;
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

}
