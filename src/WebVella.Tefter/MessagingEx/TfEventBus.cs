namespace WebVella.Tefter.MessagingEx;

public partial interface ITfEventBus
{
	IDisposable Subscribe<T>(
		Action<string?, T?> handler,
		string? key = null);

	public IDisposable Subscribe<T>(
		Func<string?, T?, ValueTask> handler,
		string? key = null);

	ValueTask<IAsyncDisposable> SubscribeAsync<T>(
		Action<string?, T?> handler,
		string? key = null);

	ValueTask<IAsyncDisposable> SubscribeAsync<T>(
	   Func<string?, T?, ValueTask> handler,
	   string? key = null);

	void Publish(
		string? key = null,
		ITfEventArgs? args = null);

	ValueTask PublishAsync(
		string? key = null,
		ITfEventArgs? args = null);
}

public partial class TfEventBus : ITfEventBus
{
	private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
	private readonly List<Subscription> _subscribers = new List<Subscription>();


	public IDisposable Subscribe<T>(
		Action<string?, T?> handler,
		string? key = null)
	{
		Type typeT = typeof(T);

		if (!typeof(ITfEventArgs).IsAssignableFrom(typeT))
			throw new ArgumentException($"Type {typeT.Name} must implement ITfEventArgs");

		ArgumentNullException.ThrowIfNull(handler);

		Action<string?, ITfEventArgs?> wrapper = (key, args) => handler(key, (T?)args);
		
		var subscription = new Subscription { 
			TargetType = typeT, 
			Key = key, 
			HandlerWrapper = wrapper, 
			IsAsync = false 
		};

		_semaphore.Wait();
		
		try 
		{ 
			_subscribers.Add(subscription); 
		}
		finally 
		{ 
			_semaphore.Release(); 
		}

		return new Unsubscriber(_subscribers, subscription, _semaphore);
	}

	public async ValueTask<IAsyncDisposable> SubscribeAsync<T>(
		Action<string?, T?> handler,
		string? key = null)
	{
		Type typeT = typeof(T);

		if (!typeof(ITfEventArgs).IsAssignableFrom(typeT))
			throw new ArgumentException($"Type {typeT.Name} must implement ITfEventArgs");

		ArgumentNullException.ThrowIfNull(handler);

		Action<string?, ITfEventArgs?> wrapper = (key, args) => handler(key, (T?)args);
		
		var subscription = new Subscription { 
			TargetType = typeT, 
			Key = key, 
			HandlerWrapper = wrapper, 
			IsAsync = false 
		};

		await _semaphore.WaitAsync();

		try
		{ 
			_subscribers.Add(subscription); 
		}
		finally 
		{
			_semaphore.Release(); 
		}

		return new Unsubscriber(_subscribers, subscription, _semaphore);
	}

	public IDisposable Subscribe<T>(
		Func<string?, T?, ValueTask> handler,
		string? key = null)
	{
		Type typeT = typeof(T);

		if (!typeof(ITfEventArgs).IsAssignableFrom(typeT))
			throw new ArgumentException($"Type {typeT.Name} must implement ITfEventArgs");

		ArgumentNullException.ThrowIfNull(handler);

		Func<string?, ITfEventArgs?, ValueTask> asyncWrapper = (k, args) => handler(k, (T?)args);
		
		var subscription = new Subscription { 
			TargetType = typeT, 
			Key = key, 
			AsyncHandlerWrapper = asyncWrapper, 
			IsAsync = true 
		};

		_semaphore.Wait();

		try 
		{
			_subscribers.Add(subscription); 
		}
		finally 
		{
			_semaphore.Release(); 
		}

		return new Unsubscriber(_subscribers, subscription, _semaphore);
	}

	public async ValueTask<IAsyncDisposable> SubscribeAsync<T>(
		Func<string?, T?, ValueTask> handler,
		string? key = null)
	{
		Type typeT = typeof(T);

		if (!typeof(ITfEventArgs).IsAssignableFrom(typeT))
			throw new ArgumentException($"Type {typeT.Name} must implement ITfEventArgs");

		ArgumentNullException.ThrowIfNull(handler);

		Func<string?, ITfEventArgs?, ValueTask> asyncWrapper = (k, args) => handler(k, (T?)args);

		var subscription = new Subscription { 
			TargetType = typeT, 
			Key = key, 
			AsyncHandlerWrapper = asyncWrapper, 
			IsAsync = true 
		};

		await _semaphore.WaitAsync();
		try 
		{
			_subscribers.Add(subscription); 
		}
		finally 
		{
			_semaphore.Release(); 
		}

		return new Unsubscriber(_subscribers, subscription, _semaphore);
	}

	public void Publish(
		string? key = null,
		ITfEventArgs? args = null)
	{
		List<Subscription> snapshot;

		_semaphore.Wait();
		try
		{
			snapshot = _subscribers.ToList();
		}
		finally
		{
			_semaphore.Release();
		}

		foreach (var sub in snapshot)
		{
			if (sub.Key is null || sub.Key == key)
			{
				bool typeMatches = (args is null) || sub.TargetType.IsAssignableFrom(args.GetType());

				if (typeMatches)
				{
					ValueTask handlerTask;
					if (sub.IsAsync)
					{
						handlerTask = sub.AsyncHandlerWrapper!(key, args);
					}
					else
					{
						// Wraps sync method in Task.Run to ensure no context is captured and offload work.
						handlerTask = new ValueTask(Task.Run(() => sub.HandlerWrapper!(key, args)));
					}

					// Safely blocks on the task using ConfigureAwait(false).
					// This minimizes deadlock risk while preserving the logic of waiting for completion.
					handlerTask.AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
				}
			}
		}
	}

	public async ValueTask PublishAsync(
		string? key = null,
		ITfEventArgs? args = null)
	{
		List<Subscription> subscriptionsSnapshot;

		await _semaphore.WaitAsync();

		try 
		{
			subscriptionsSnapshot = _subscribers.ToList(); 
		}
		finally 
		{
			_semaphore.Release(); 
		}

		List<ValueTask> tasks = new List<ValueTask>();

		foreach (var subscription in subscriptionsSnapshot)
		{
			if (subscription.Key is null || subscription.Key == key)
			{
				bool typeMatches = (args is null) || subscription.TargetType.IsAssignableFrom(args.GetType());

				if (typeMatches)
				{
					if (subscription.IsAsync)
					{
						tasks.Add(subscription.AsyncHandlerWrapper!(key, args));
					}
					else
					{
						tasks.Add(new ValueTask(Task.Run(() => subscription.HandlerWrapper!(key, args))));
					}
				}
			}
		}

		await Task.WhenAll(tasks.Select(vt => vt.AsTask()));
	}


	private class Subscription
	{
		public required Type TargetType { get; set; }
		public string? Key { get; set; } = null;
		public Action<string?, ITfEventArgs?>? HandlerWrapper { get; set; }
		public Func<string?, ITfEventArgs?, ValueTask>? AsyncHandlerWrapper { get; set; }
		public required bool IsAsync { get; set; }
	}

	private class Unsubscriber : IDisposable, IAsyncDisposable
	{
		private readonly List<Subscription> _subscribers;
		private readonly Subscription _subscription;
		private readonly SemaphoreSlim _semaphore;

		public Unsubscriber(
			List<Subscription> subscribers,
			Subscription subscription,
			SemaphoreSlim semaphore)
		{
			_subscribers = subscribers;
			_subscription = subscription;
			_semaphore = semaphore;
		}

		public void Dispose()
		{
			_semaphore.Wait();

			try
			{
				if (_subscribers.Contains(_subscription))
				{
					_subscribers.Remove(_subscription);
				}
			}
			finally
			{
				_semaphore.Release();
			}
		}

		public async ValueTask DisposeAsync()
		{
			await _semaphore.WaitAsync();

			try
			{
				if (_subscribers.Contains(_subscription))
				{
					_subscribers.Remove(_subscription);
				}
			}
			finally
			{
				_semaphore.Release();
			}
		}
	}
}