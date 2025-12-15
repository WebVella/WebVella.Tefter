namespace WebVella.Tefter.MessagingEx;

public partial interface ITfEventBus
{
	IDisposable Subscribe<T>(
		Action<T?> handler,
		string? key = null);
	ValueTask<IAsyncDisposable> SubscribeAsync<T>(
		Action<T?> handler,
		string? key = null);
	void Publish(
		string? key = null,
		ITfEventPayload? payload = null);
	ValueTask PublishAsync(
		string? key = null,
		ITfEventPayload? payload = null);
}

public partial class TfEventBus : ITfEventBus
{
	private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
	private readonly List<Subscription> _subscribers = new List<Subscription>();

	public IDisposable Subscribe<T>(
		Action<T?> handler,
		string? key = null)
	{
		Type typeT = typeof(T);

		if (!typeof(ITfEventPayload).IsAssignableFrom(typeT))
			throw new ArgumentException($"Type {typeT.Name} must implement ITfEventPayload");

		if (handler == null) throw new ArgumentNullException(nameof(handler));


		Action<ITfEventPayload?> wrapper = (payload) => handler((T?)payload);

		var subscription = new Subscription
		{
			TargetType = typeT,
			Key = key,
			HandlerWrapper = wrapper
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
		Action<T?> handler,
		string? key = null)
	{
		Type typeT = typeof(T);

		if (!typeof(ITfEventPayload).IsAssignableFrom(typeT))
			throw new ArgumentException($"Type {typeT.Name} must implement ITfEventPayload");

		if (handler == null) throw new ArgumentNullException(nameof(handler));


		Action<ITfEventPayload?> wrapper = (payload) => handler((T?)payload);

		var subscription = new Subscription
		{
			TargetType = typeT,
			Key = key,
			HandlerWrapper = wrapper
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
		ITfEventPayload? payload = null)
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
				if (payload is null)
				{
					sub.HandlerWrapper(payload);
				}
				else if (sub.TargetType.IsAssignableFrom(payload.GetType()))
				{
					sub.HandlerWrapper(payload);
				}
			}
		}
	}

	public async ValueTask PublishAsync(
		string? key = null,
		ITfEventPayload? payload = null)
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

		foreach (var subscription in subscriptionsSnapshot)
		{
			if (subscription.Key is null || subscription.Key == key)
			{
				if (payload is null)
				{
					subscription.HandlerWrapper(payload);
				}
				else if (subscription.TargetType.IsAssignableFrom(payload.GetType()))
				{
					subscription.HandlerWrapper(payload);
				}
			}
		}
	}

	private class Subscription
	{
		public required Type TargetType { get; set; }
		public string? Key { get; set; } = null;
		public required Action<ITfEventPayload?> HandlerWrapper { get; set; }
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