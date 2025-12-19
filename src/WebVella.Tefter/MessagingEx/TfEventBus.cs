namespace WebVella.Tefter.MessagingEx;

public partial interface ITfEventBus
{
	public IDisposable Subscribe<T>(
		Action<string?, T?> handler,
		Guid key);

	public IDisposable Subscribe<T>(
		Action<string?, T?> handler,
		string? key = null);

	public IDisposable Subscribe<T>(
		Func<string?, T?, ValueTask> handler,
		Guid key);

	public IDisposable Subscribe<T>(
		Func<string?, T?, ValueTask> handler,
		string? key = null);

	public ValueTask<IAsyncDisposable> SubscribeAsync<T>(
		Action<string?, T?> handler,
		Guid key);

	public ValueTask<IAsyncDisposable> SubscribeAsync<T>(
		Action<string?, T?> handler,
		string? key = null);

	public ValueTask<IAsyncDisposable> SubscribeAsync<T>(
		Func<string?, T?, ValueTask> handler,
		Guid key);

	public ValueTask<IAsyncDisposable> SubscribeAsync<T>(
	   Func<string?, T?, ValueTask> handler,
	   string? key = null);

	public void Publish(
		Guid key,
		ITfEventPayload? args = null);

	public void Publish(
		string? key = null,
		ITfEventPayload? args = null);

	public ValueTask PublishAsync(
		Guid key,
		ITfEventPayload? args = null);

	public ValueTask PublishAsync(
		string? key = null,
		ITfEventPayload? args = null);
}

public partial class TfEventBus : ITfEventBus
{
	private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
	private readonly List<Subscription> _subscribers = new List<Subscription>();

	public IDisposable Subscribe<T>(
		Action<string?, T?> handler,
		Guid key)
	{
		return Subscribe(handler, key.ToString().ToLowerInvariant());
	}

	public IDisposable Subscribe<T>(
		Action<string?, T?> handler,
		string? key = null)
	{
		Type typeT = typeof(T);

		if (!typeof(ITfEventPayload).IsAssignableFrom(typeT))
			throw new ArgumentException($"Type {typeT.Name} must implement ITfEventPayload");

		ArgumentNullException.ThrowIfNull(handler);

		Action<string?, ITfEventPayload?> wrapper = (key, args) => handler(key, (T?)args);
		
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
		Guid key)
	{
		return await SubscribeAsync(handler, key.ToString().ToLowerInvariant());	
	}

	public async ValueTask<IAsyncDisposable> SubscribeAsync<T>(
		Action<string?, T?> handler,
		string? key = null)
	{
		Type typeT = typeof(T);

		if (!typeof(ITfEventPayload).IsAssignableFrom(typeT))
			throw new ArgumentException($"Type {typeT.Name} must implement ITfEventPayload");

		ArgumentNullException.ThrowIfNull(handler);

		Action<string?, ITfEventPayload?> wrapper = (key, args) => handler(key, (T?)args);
		
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
		Guid key)
	{
		return Subscribe(handler, key.ToString().ToLowerInvariant());
	}

	public IDisposable Subscribe<T>(
		Func<string?, T?, ValueTask> handler,
		string? key = null)
	{
		Type typeT = typeof(T);

		if (!typeof(ITfEventPayload).IsAssignableFrom(typeT))
			throw new ArgumentException($"Type {typeT.Name} must implement ITfEventPayload");

		ArgumentNullException.ThrowIfNull(handler);

		Func<string?, ITfEventPayload?, ValueTask> asyncWrapper = (k, args) => handler(k, (T?)args);
		
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
		Guid key)
	{
		return await SubscribeAsync(handler, key.ToString().ToLowerInvariant());
	}

	public async ValueTask<IAsyncDisposable> SubscribeAsync<T>(
		Func<string?, T?, ValueTask> handler,
		string? key = null)
	{
		Type typeT = typeof(T);

		if (!typeof(ITfEventPayload).IsAssignableFrom(typeT))
			throw new ArgumentException($"Type {typeT.Name} must implement ITfEventPayload");

		ArgumentNullException.ThrowIfNull(handler);

		Func<string?, ITfEventPayload?, ValueTask> asyncWrapper = (k, args) => handler(k, (T?)args);

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
		Guid key,
		ITfEventPayload? args = null)
	{
		Publish(key.ToString().ToLowerInvariant(), args);
	}

	public void Publish(
		string? key = null,
		ITfEventPayload? args = null)
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
		Guid key,
		ITfEventPayload? args = null)
	{
		await PublishAsync(key.ToString().ToLowerInvariant(), args);
	}

	public async ValueTask PublishAsync(
		string? key = null,
		ITfEventPayload? args = null)
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
		public Action<string?, ITfEventPayload?>? HandlerWrapper { get; set; }
		public Func<string?, ITfEventPayload?, ValueTask>? AsyncHandlerWrapper { get; set; }
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