namespace WebVella.Tefter.UI.EventsBus;

public interface ITfEventBus
{
	IDisposable Subscribe<T>(
		Action<string?, T?> 
		handler, Func<string?, bool> matchKey);

	IDisposable Subscribe<T>(
		Func<string?, T?, Task> handler,
		Func<string?, bool> matchKey);

	IDisposable Subscribe<T>(
		Action<string?, T?> handler,
		Guid key);

	IDisposable Subscribe<T>(
		Action<string?, T?> handler,
		string? key = null);

	IDisposable Subscribe<T>(
		Func<string?, T?, Task> handler,
		Guid key);

	IDisposable Subscribe<T>(
		Func<string?, T?, Task> handler,
		string? key = null);

	Task<IAsyncDisposable> SubscribeAsync<T>(
		Action<string?, T?> handler,
		Func<string?, bool> matchKey);

	Task<IAsyncDisposable> SubscribeAsync<T>(
		Func<string?, T?, Task> handler,
		Func<string?, bool> matchKey);

	Task<IAsyncDisposable> SubscribeAsync<T>(
		Action<string?, T?> handler,
		Guid key);

	Task<IAsyncDisposable> SubscribeAsync<T>(
		Action<string?, T?> handler,
		string? key = null);

	Task<IAsyncDisposable> SubscribeAsync<T>(
		Func<string?, T?, Task> handler,
		Guid key);

	Task<IAsyncDisposable> SubscribeAsync<T>(
		Func<string?, T?, Task> handler,
		string? key = null);

	void Publish(
		Guid key,
		ITfEventPayload? payload = null);

	void Publish(
		string? key = null,
		ITfEventPayload? payload = null);

	Task PublishAsync(
		Guid key,
		ITfEventPayload? payload = null);

	Task PublishAsync(
		string? key = null,
		ITfEventPayload? payload = null);
}

public class TfEventBus : ITfEventBus
{
	private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
	private readonly List<Subscription> _subscribers = new List<Subscription>();

	#region Subscribe Sync

	public IDisposable Subscribe<T>(Action<string?, T?> handler, Guid key)
		=> Subscribe(handler, key.ToString().ToLowerInvariant());

	public IDisposable Subscribe<T>(Action<string?, T?> handler, string? key = null)
	{
		return Subscribe<T>(handler, publishedKey => key is null || publishedKey == key);
	}

	public IDisposable Subscribe<T>(Action<string?, T?> handler, Func<string?, bool> matchKey)
	{
		Type typeT = typeof(T);
		if (!typeof(ITfEventPayload).IsAssignableFrom(typeT))
			throw new ArgumentException($"Type {typeT.Name} must implement ITfEventPayload");

		ArgumentNullException.ThrowIfNull(handler);

		void Wrapper(string? k, ITfEventPayload? args) => handler(k, (T?)args);

		var subscription = new Subscription
		{
			TargetType = typeT,
			KeyPredicate = matchKey,
			HandlerWrapper = Wrapper, 
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
		return Subscribe<T>(handler, publishedKey => key is null || publishedKey == key);
	}

	public IDisposable Subscribe<T>(Func<string?, T?, Task> handler, Func<string?, bool> matchKey)
	{
		Type typeT = typeof(T);
		if (!typeof(ITfEventPayload).IsAssignableFrom(typeT))
			throw new ArgumentException($"Type {typeT.Name} must implement ITfEventPayload");

		ArgumentNullException.ThrowIfNull(handler);

		Task AsyncWrapper(string? k, ITfEventPayload? args) => handler(k, (T?)args);

		var subscription = new Subscription
		{
			TargetType = typeT,
			KeyPredicate = matchKey,
			AsyncHandlerWrapper = AsyncWrapper, 
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
		return await SubscribeAsync<T>(handler, publishedKey => key is null || publishedKey == key);
	}

	public async Task<IAsyncDisposable> SubscribeAsync<T>(Action<string?, T?> handler, Func<string?, bool> matchKey)
	{
		Type typeT = typeof(T);
		if (!typeof(ITfEventPayload).IsAssignableFrom(typeT))
			throw new ArgumentException($"Type {typeT.Name} must implement ITfEventPayload");

		ArgumentNullException.ThrowIfNull(handler);

		void Wrapper(string? k, ITfEventPayload? args) => handler(k, (T?)args);

		var subscription = new Subscription
		{
			TargetType = typeT,
			KeyPredicate = matchKey,
			HandlerWrapper = Wrapper, 
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
		return await SubscribeAsync<T>(handler, publishedKey => key is null || publishedKey == key);
	}

	public async Task<IAsyncDisposable> SubscribeAsync<T>(Func<string?, T?, Task> handler, Func<string?, bool> matchKey)
	{
		Type typeT = typeof(T);
		if (!typeof(ITfEventPayload).IsAssignableFrom(typeT))
			throw new ArgumentException($"Type {typeT.Name} must implement ITfEventPayload");

		ArgumentNullException.ThrowIfNull(handler);

		Task AsyncWrapper(string? k, ITfEventPayload? args) => handler(k, (T?)args);

		var subscription = new Subscription
		{
			TargetType = typeT,
			KeyPredicate = matchKey, 
			AsyncHandlerWrapper = AsyncWrapper, 
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
			if (sub.KeyPredicate(key))
			{
				// ReSharper disable once CanSimplifyIsAssignableFrom
				bool typeMatches = (args is null) || sub.TargetType.IsAssignableFrom(args.GetType());
				if (typeMatches)
				{
					if (sub.IsAsync)
					{
						_ = RunSafeAsync(() => sub.AsyncHandlerWrapper!(key, args));
					}
					else
					{
						_= Task.Run(() =>
						{
							try { sub.HandlerWrapper!(key, args); }
							catch (Exception)
							{
								//Ignore
							}
						});
					}					
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

		foreach (var subscription in snapshot)
		{
			if (subscription.KeyPredicate(key))
			{
				// ReSharper disable once CanSimplifyIsAssignableFrom
				bool typeMatches = (args is null) || subscription.TargetType.IsAssignableFrom(args.GetType());
				if (typeMatches)
				{
					if (subscription.IsAsync)
					{
						_ = RunSafeAsync(() => subscription.AsyncHandlerWrapper!(key, args));
					}
					else
					{
						_= Task.Run(() =>
						{
							try { subscription.HandlerWrapper!(key, args); }
							catch (Exception)
							{
								//Ignore
							}
						});
					}
				}
			}
		}
	}

	static Task RunSafeAsync(Func<Task> handler)
	{
		try { _ = handler(); }
		catch (Exception)
		{
			//Ignore
		}

		return Task.CompletedTask;
	}

	#endregion

	private class Subscription
	{
		public required Type TargetType { get; init; }
		public required Func<string?, bool> KeyPredicate { get; init; }
		public Action<string?, ITfEventPayload?>? HandlerWrapper { get; init; }
		public Func<string?, ITfEventPayload?, Task>? AsyncHandlerWrapper { get; init; }
		public required bool IsAsync { get; init; }
	}

	private class Unsubscriber(List<Subscription> subscribers, Subscription subscription, SemaphoreSlim semaphore)
		: IDisposable, IAsyncDisposable
	{
		public void Dispose()
		{
			semaphore.Wait();
			try { subscribers.Remove(subscription); }
			finally { semaphore.Release(); }
		}

		public async ValueTask DisposeAsync()
		{
			await semaphore.WaitAsync();
			try { subscribers.Remove(subscription); }
			finally { semaphore.Release(); }
		}
	}
}