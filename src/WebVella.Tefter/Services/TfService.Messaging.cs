using Nito.AsyncEx.Synchronous;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	void PublishEventWithScope(IGlobalEvent eventObj);
	Task PublishEventWithScopeAsync(IGlobalEvent eventObj);
}

public partial class TfService : ITfService
{
	public void PublishEventWithScope(IGlobalEvent eventObj)
	{
		using (var scope = _serviceProvider.CreateScope())
		{
			var task = Task.Run(async () =>
			{
				var eventProvider = scope.ServiceProvider.GetRequiredService<TfGlobalEventProvider>();
				await eventProvider.PublishEventAsync(eventObj);
			});
			task.WaitAndUnwrapException();
		}
	}

	public async Task PublishEventWithScopeAsync(IGlobalEvent eventObj)
	{
		using (var scope = _serviceProvider.CreateScope())
		{
			var eventProvider = scope.ServiceProvider.GetRequiredService<TfGlobalEventProvider>();
			await eventProvider.PublishEventAsync(eventObj);
		}
	}
}