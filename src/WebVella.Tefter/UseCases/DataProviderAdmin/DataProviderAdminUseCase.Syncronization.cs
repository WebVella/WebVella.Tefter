namespace WebVella.Tefter.UseCases.DataProviderAdmin;
public partial class DataProviderAdminUseCase
{
	internal TucDataProviderSyncTask LastNotCompletedSyncTask { get; set; } = null;
	internal TucDataProviderSyncTask LastCompletedSyncTask { get; set; } = null;

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
		var lastNotCompleted = tasks.FirstOrDefault(x=> x.Status == TfSynchronizationStatus.Pending || x.Status == TfSynchronizationStatus.InProgress);
		var lastCompleted = tasks.FirstOrDefault(x=> x.Status == TfSynchronizationStatus.Completed || x.Status == TfSynchronizationStatus.Failed);
		LastNotCompletedSyncTask = lastNotCompleted is null ? null : new TucDataProviderSyncTask(lastNotCompleted);
		LastCompletedSyncTask = lastCompleted is null ? null : new TucDataProviderSyncTask(lastCompleted);

		return Task.CompletedTask;
	}

}
