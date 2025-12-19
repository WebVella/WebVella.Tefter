namespace WebVella.Tefter.MessagingEx;

public partial interface ITfEventBusEx
{
	public IDisposable Subscribe<T>(
		Action<string?, T?> handler,
		Guid key);

	public IDisposable Subscribe<T>(
		Action<string?, T?> handler,
		string? key = null);

	public IDisposable Subscribe<T>(
		Func<string?, T?, Task> handler,
		Guid key);

	public IDisposable Subscribe<T>(
		Func<string?, T?, Task> handler,
		string? key = null);

	public Task<IAsyncDisposable> SubscribeAsync<T>(
		Action<string?, T?> handler,
		Guid key);

	public Task<IAsyncDisposable> SubscribeAsync<T>(
		Action<string?, T?> handler,
		string? key = null);

	public Task<IAsyncDisposable> SubscribeAsync<T>(
		Func<string?, T?, Task> handler,
		Guid key);

	public Task<IAsyncDisposable> SubscribeAsync<T>(
	   Func<string?, T?, Task> handler,
	   string? key = null);

	public void Publish(
		Guid key,
		ITfEventPayload? payload = null);

	public void Publish(
		string? key = null,
		ITfEventPayload? payload = null);

	public Task PublishAsync(
		Guid key,
		ITfEventPayload? payload = null);

	public Task PublishAsync(
		string? key = null,
		ITfEventPayload? payload = null);
}

public partial class TfEventBusEx : ITfEventBusEx
{
	private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
	private readonly List<Subscription> _subscribers = new List<Subscription>();

	#region Subscribe Sync
	public IDisposable Subscribe<T>(Action<string?, T?> handler, Guid key)
		=> Subscribe(handler, key.ToString().ToLowerInvariant());

	public IDisposable Subscribe<T>(Action<string?, T?> handler, string? key = null)
	{
		Type typeT = typeof(T);
		if (!typeof(ITfEventPayload).IsAssignableFrom(typeT))
			throw new ArgumentException($"Type {typeT.Name} must implement ITfEventPayload");

		ArgumentNullException.ThrowIfNull(handler);

		Action<string?, ITfEventPayload?> wrapper = (k, args) => handler(k, (T?)args);

		var subscription = new Subscription
		{
			TargetType = typeT,
			Key = key,
			HandlerWrapper = wrapper,
			IsAsync = false
		};

		_semaphore.Wait();
		try { _subscribers.Add(subscription); }
		finally { _semaphore.Release(); }

		return new Unsubscriber(_subscribers, subscription, _semaphore);
	}

	public IDisposable Subscribe<T>(Func<string?, T?, Task> handler, Guid key)
		=> Subscribe(handler, key.ToString().ToLowerInvariant());

	public IDisposable Subscribe<T>(Func<string?, T?, Task> handler, string? key = null)
	{
		Type typeT = typeof(T);
		if (!typeof(ITfEventPayload).IsAssignableFrom(typeT))
			throw new ArgumentException($"Type {typeT.Name} must implement ITfEventPayload");

		ArgumentNullException.ThrowIfNull(handler);

		Func<string?, ITfEventPayload?, Task> asyncWrapper = (k, args) => handler(k, (T?)args);

		var subscription = new Subscription
		{
			TargetType = typeT,
			Key = key,
			AsyncHandlerWrapper = asyncWrapper,
			IsAsync = true
		};

		_semaphore.Wait();
		try { _subscribers.Add(subscription); }
		finally { _semaphore.Release(); }

		return new Unsubscriber(_subscribers, subscription, _semaphore);
	}
	#endregion

	#region Subscribe Async
	public async Task<IAsyncDisposable> SubscribeAsync<T>(Action<string?, T?> handler, Guid key)
		=> await SubscribeAsync(handler, key.ToString().ToLowerInvariant());

	public async Task<IAsyncDisposable> SubscribeAsync<T>(Action<string?, T?> handler, string? key = null)
	{
		Type typeT = typeof(T);
		if (!typeof(ITfEventPayload).IsAssignableFrom(typeT))
			throw new ArgumentException($"Type {typeT.Name} must implement ITfEventPayload");

		ArgumentNullException.ThrowIfNull(handler);

		Action<string?, ITfEventPayload?> wrapper = (k, args) => handler(k, (T?)args);

		var subscription = new Subscription
		{
			TargetType = typeT,
			Key = key,
			HandlerWrapper = wrapper,
			IsAsync = false
		};

		await _semaphore.WaitAsync();
		try { _subscribers.Add(subscription); }
		finally { _semaphore.Release(); }

		return new Unsubscriber(_subscribers, subscription, _semaphore);
	}

	public async Task<IAsyncDisposable> SubscribeAsync<T>(Func<string?, T?, Task> handler, Guid key)
		=> await SubscribeAsync(handler, key.ToString().ToLowerInvariant());

	public async Task<IAsyncDisposable> SubscribeAsync<T>(Func<string?, T?, Task> handler, string? key = null)
	{
		Type typeT = typeof(T);
		if (!typeof(ITfEventPayload).IsAssignableFrom(typeT))
			throw new ArgumentException($"Type {typeT.Name} must implement ITfEventPayload");

		ArgumentNullException.ThrowIfNull(handler);

		Func<string?, ITfEventPayload?, Task> asyncWrapper = (k, args) => handler(k, (T?)args);

		var subscription = new Subscription
		{
			TargetType = typeT,
			Key = key,
			AsyncHandlerWrapper = asyncWrapper,
			IsAsync = true
		};

		await _semaphore.WaitAsync();
		try { _subscribers.Add(subscription); }
		finally { _semaphore.Release(); }

		return new Unsubscriber(_subscribers, subscription, _semaphore);
	}
	#endregion

	#region Publish
	public void Publish(Guid key, ITfEventPayload? args = null)
		=> Publish(key.ToString().ToLowerInvariant(), args);

	public void Publish(string? key = null, ITfEventPayload? args = null)
	{
		List<Subscription> snapshot;
		_semaphore.Wait();
		try { snapshot = _subscribers.ToList(); }
		finally { _semaphore.Release(); }

		foreach (var sub in snapshot)
		{
			if (sub.Key is null || sub.Key == key)
			{
				bool typeMatches = (args is null) || sub.TargetType.IsAssignableFrom(args.GetType());
				if (typeMatches)
				{
					Task handlerTask = sub.IsAsync
						? sub.AsyncHandlerWrapper!(key, args)
						: Task.Run(() => sub.HandlerWrapper!(key, args));

					// Blocking wait for Task
					handlerTask.ConfigureAwait(false).GetAwaiter().GetResult();
				}
			}
		}
	}

	public async Task PublishAsync(Guid key, ITfEventPayload? args = null)
		=> await PublishAsync(key.ToString().ToLowerInvariant(), args);

	public async Task PublishAsync(string? key = null, ITfEventPayload? args = null)
	{
		List<Subscription> snapshot;
		await _semaphore.WaitAsync();
		try { snapshot = _subscribers.ToList(); }
		finally { _semaphore.Release(); }

		List<Task> tasks = new List<Task>();

		foreach (var subscription in snapshot)
		{
			if (subscription.Key is null || subscription.Key == key)
			{
				bool typeMatches = (args is null) || subscription.TargetType.IsAssignableFrom(args.GetType());
				if (typeMatches)
				{
					tasks.Add(subscription.IsAsync
						? subscription.AsyncHandlerWrapper!(key, args)
						: Task.Run(() => subscription.HandlerWrapper!(key, args)));
				}
			}
		}

		await Task.WhenAll(tasks);
	}
	#endregion

	private class Subscription
	{
		public required Type TargetType { get; set; }
		public string? Key { get; set; }
		public Action<string?, ITfEventPayload?>? HandlerWrapper { get; set; }
		public Func<string?, ITfEventPayload?, Task>? AsyncHandlerWrapper { get; set; }
		public required bool IsAsync { get; set; }
	}

	private class Unsubscriber : IDisposable, IAsyncDisposable
	{
		private readonly List<Subscription> _subscribers;
		private readonly Subscription _subscription;
		private readonly SemaphoreSlim _semaphore;

		public Unsubscriber(List<Subscription> subscribers, Subscription subscription, SemaphoreSlim semaphore)
		{
			_subscribers = subscribers;
			_subscription = subscription;
			_semaphore = semaphore;
		}

		public void Dispose()
		{
			_semaphore.Wait();
			try { _subscribers.Remove(_subscription); }
			finally { _semaphore.Release(); }
		}

		public async ValueTask DisposeAsync()
		{
			await _semaphore.WaitAsync();
			try { _subscribers.Remove(_subscription); }
			finally { _semaphore.Release(); }
		}
	}
}