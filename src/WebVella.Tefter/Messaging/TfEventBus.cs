using System.Diagnostics;

namespace WebVella.Tefter.Messaging;

public interface ITfEventBus: IAsyncDisposable
{
	event Action<ITfEvent> OnEvent;
	Task JoinChannelsAsync(params string[] channels);
	Task LeaveChannelsAsync(params string[] channels);
	Task LeaveAllChannelsAsync();
	Task PublishEventAsync(ITfEvent tfEvent);
}


public class TfEventBus : ITfEventBus, IAsyncDisposable
{
	private readonly IChannelEventRouter _channelEventRouter;

	public event Action<ITfEvent> OnEvent;

	public TfEventBus(IServiceProvider serviceProvider)
	{
		_channelEventRouter = serviceProvider.GetRequiredService<IChannelEventRouter>();
	}

	internal async Task RaiseEventInternal(ITfEvent tfEvent)
	{
		await Task.Run(() => { try { OnEvent?.Invoke(tfEvent); } catch {} });
	}

	public async Task PublishEventAsync(ITfEvent tfEvent)
	{
		await _channelEventRouter.PublishAsync(this, tfEvent);
	}

	public async Task JoinChannelsAsync(params string[] channels)
	{
		await _channelEventRouter.JoinChannelsAsync(this, channels);
	}

	public async Task LeaveChannelsAsync(params string[] channels)
	{
		await _channelEventRouter.LeaveChannelsAsync(this, channels);
	}

	public async Task LeaveAllChannelsAsync()
	{
		await _channelEventRouter.LeaveAllChannelsAsync(this);
	}

	public async ValueTask DisposeAsync()
	{
		//cleanup so GC can recycle it if there are events subscribed
		await LeaveAllChannelsAsync();
		OnEvent = null;
	}
}
