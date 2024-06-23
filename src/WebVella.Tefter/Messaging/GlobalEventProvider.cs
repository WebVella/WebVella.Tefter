namespace WebVella.Tefter.Messaging;

public interface IGlobalEvent : ITfEvent { }

public partial class GlobalEventProvider : IAsyncDisposable
{
	private const string GLOBAL_CHANNEL = "GLOBAL_CHANNEL";
	private readonly ITfEventBus _eventBus;

	public GlobalEventProvider(ITfEventBus eventBus)
	{
		_eventBus = eventBus;
		_eventBus.JoinChannelsAsync(GLOBAL_CHANNEL);
		_eventBus.OnEvent += OnEventReceived;
	}

	public async Task PublishEventAsync(IGlobalEvent globalEvent)
	{
		await _eventBus.PublishEventAsync(globalEvent);
	}

	public async ValueTask DisposeAsync()
	{
		await _eventBus.DisposeAsync();
	}
}
