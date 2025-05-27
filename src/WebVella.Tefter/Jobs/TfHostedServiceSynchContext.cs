namespace WebVella.Tefter;

public class TfHostedServiceSynchContext : SynchronizationContext
{
	private readonly BlockingCollection<(SendOrPostCallback Callback, object State)> _queue =
		new BlockingCollection<(SendOrPostCallback, object)>();

	private readonly Thread _thread;

	public TfHostedServiceSynchContext()
	{
		_thread = new Thread(RunOnCurrentThread) { IsBackground = true };
		_thread.Start();
	}

	public override void Post(SendOrPostCallback d, object state)
	{
		_queue.Add((d, state));
	}

	public override void Send(SendOrPostCallback callback, object state)
	{
		if (Thread.CurrentThread == _thread)
		{
			callback(state);
		}
		else
		{
			using (var waitHandle = new ManualResetEvent(false))
			{
				_queue.Add((s =>
				{
					callback(s);
					waitHandle.Set();
				}, state));
				waitHandle.WaitOne();
			}
		}
	}

	private void RunOnCurrentThread()
	{
		foreach (var (callback, state) in _queue.GetConsumingEnumerable())
		{
			callback(state);
		}
	}

	public void Complete()
	{
		_queue.CompleteAdding();
	}
}
