namespace WebVella.Tefter.UseCases.DataProviderAdmin;
public partial class DataProviderAdminUseCase
{
	internal IQueryable<TucDataProviderSyncTaskInfo> TaskSyncLogRecords { get; set; } = Enumerable.Empty<TucDataProviderSyncTaskInfo>().AsQueryable();
	internal int TaskSyncLogPageSize { get; set; } = 15;
	internal Task InitSyncLogDialog()
	{
		return Task.CompletedTask;
	}

	internal Task SetSynchronizationTaskLogRecords(Guid taskId,
		TucDataProviderSyncTaskInfoType type)
	{
		var allTasksResult = _dataProviderManager.GetSynchronizationTaskResultInfos(taskId);
		if (allTasksResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSynchronizationTaskResultInfos failed").CausedBy(allTasksResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.CompletedTask;
		}

		switch (type)
		{
			case TucDataProviderSyncTaskInfoType.Info:
				TaskSyncLogRecords = allTasksResult.Value.Where(x => !String.IsNullOrWhiteSpace(x.Info))
				.Select(x => new TucDataProviderSyncTaskInfo(x)).AsQueryable();
				break;
			case TucDataProviderSyncTaskInfoType.Warning:
				TaskSyncLogRecords = allTasksResult.Value.Where(x => !String.IsNullOrWhiteSpace(x.Warning))
				.Select(x => new TucDataProviderSyncTaskInfo(x)).AsQueryable();
				break;
			case TucDataProviderSyncTaskInfoType.Error:
				TaskSyncLogRecords = allTasksResult.Value.Where(x => !String.IsNullOrWhiteSpace(x.Error))
				.Select(x => new TucDataProviderSyncTaskInfo(x)).AsQueryable();
				break;
			default:
				throw new Exception("Not supported TucDataProviderSyncTaskInfoType");
		}
		return Task.CompletedTask;
	}

}
