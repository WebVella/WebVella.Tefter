namespace WebVella.Tefter.Messaging;

internal interface ITfChannelEventRouter
{
	Task JoinChannelsAsync(ITfEventBus bus, params string[] channels);
	Task LeaveChannelsAsync(ITfEventBus bus, params string[] channels);
	Task LeaveAllChannelsAsync(ITfEventBus bus);
	Task PublishAsync(ITfEventBus bus, ITfEvent tfEvent);
	void LeaveAllChannels(ITfEventBus bus);
}


internal sealed class TfChannelEventRouter : ITfChannelEventRouter
{
	private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
	private readonly Dictionary<string, HashSet<ITfEventBus>> _channelDict;
	private readonly Dictionary<ITfEventBus, HashSet<string>> _busDict;

	public TfChannelEventRouter()
	{
		_channelDict = new Dictionary<string, HashSet<ITfEventBus>>();
		_busDict = new Dictionary<ITfEventBus, HashSet<string>>();
	}

	public async Task PublishAsync(ITfEventBus bus, ITfEvent tfEvent)
	{
		await _semaphore.WaitAsync();
		try
		{
			//throw probably
			if (!_busDict.ContainsKey(bus))
				return;

			foreach (var channel in _busDict[bus])
			{
				foreach (var eventBus in _channelDict[channel])
					await ((TfEventBus)eventBus).RaiseEventInternal(tfEvent);
			}

		}
		finally
		{
			_semaphore.Release();
		}
	}

	public async Task JoinChannelsAsync(ITfEventBus bus, params string[] channels)
	{
		if (bus == null || channels == null || channels.Length == 0)
			return;

		var processedChannels = channels.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
		if (processedChannels.Count == 0)
			return;

		await _semaphore.WaitAsync();
		try
		{
			if (!_busDict.ContainsKey(bus))
				_busDict[bus] = new HashSet<string>();

			foreach (var channel in processedChannels)
			{
				if (!_busDict[bus].Contains(channel))
					_busDict[bus].Add(channel);

				if (!_channelDict.ContainsKey(channel))
					_channelDict[channel] = new HashSet<ITfEventBus>();

				if (!_channelDict[channel].Contains(bus))
					_channelDict[channel].Add(bus);
			}
		}
		finally
		{
			_semaphore.Release();
		}
	}

	public async Task LeaveChannelsAsync(ITfEventBus bus, params string[] channels)
	{
		if (bus == null || channels == null || channels.Length == 0)
			return;

		var processedChannels = channels.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
		if (processedChannels.Count == 0)
			return;

		await _semaphore.WaitAsync();
		try
		{
			if (!_busDict.ContainsKey(bus))
				return;

			foreach (var channel in processedChannels)
			{
				if (!_busDict[bus].Contains(channel))
					continue;

				_busDict[bus].Remove(channel);
				_channelDict[channel].Remove(bus);

				if (_channelDict[channel].Count == 0)
					_channelDict.Remove(channel);
			}

			if (_busDict[bus].Count == 0)
				_busDict.Remove(bus);
		}
		finally
		{
			_semaphore.Release();
		}
	}

	public async Task LeaveAllChannelsAsync(ITfEventBus bus)
	{
		await _semaphore.WaitAsync();
		try
		{
			if (!_busDict.ContainsKey(bus))
				return;

			foreach (var channel in _busDict[bus])
			{
				_channelDict[channel].Remove(bus);

				if (_channelDict[channel].Count == 0)
					_channelDict.Remove(channel);
			}
			_busDict.Remove(bus);
		}
		finally
		{
			_semaphore.Release();
		}
	}

	public void LeaveAllChannels(ITfEventBus bus)
	{
		_semaphore.Wait();
		try
		{
			if (!_busDict.ContainsKey(bus))
				return;

			foreach (var channel in _busDict[bus])
			{
				_channelDict[channel].Remove(bus);

				if (_channelDict[channel].Count == 0)
					_channelDict.Remove(channel);
			}
			_busDict.Remove(bus);
		}
		finally
		{
			_semaphore.Release();
		}
	}
}