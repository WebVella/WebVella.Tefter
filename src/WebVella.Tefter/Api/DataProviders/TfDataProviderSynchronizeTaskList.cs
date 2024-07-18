namespace WebVella.Tefter;

public interface ITfDataProviderSynchronizeTaskList
{
	void AddTask(TfDataProvider provider);
	TfDataProviderSynchronizeTask GetPendingTask();
}

public class TfDataProviderSynchronizeTaskList : ITfDataProviderSynchronizeTaskList
{
	private static AsyncLock _lock;
	private List<TfDataProviderSynchronizeTask> _tasks;

	public TfDataProviderSynchronizeTaskList()
	{
		_lock = new AsyncLock();
		_tasks = new List<TfDataProviderSynchronizeTask>();
	}

	public void AddTask(TfDataProvider provider)
	{
		using (_lock.Lock())
		{
			TfDataProviderSynchronizeTask task = new TfDataProviderSynchronizeTask
			{
				DataProvider = provider,
				CreatedOn = DateTime.Now,
				Status = TfSynchronizationStatus.Pending
			};
			_tasks.Add(task);
		}
	}

	public TfDataProviderSynchronizeTask GetPendingTask()
	{
		using (_lock.Lock())
		{
			return _tasks
				.OrderBy(x => x.CreatedOn)
				.FirstOrDefault(x => x.Status == TfSynchronizationStatus.Pending);
		}
	}
}
