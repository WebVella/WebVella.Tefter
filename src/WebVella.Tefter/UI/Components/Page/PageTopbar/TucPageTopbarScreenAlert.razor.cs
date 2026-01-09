namespace WebVella.Tefter.UI.Components;

public partial class TucPageTopbarScreenAlert : TfBaseComponent, IAsyncDisposable
{
	private IAsyncDisposable? _pageOutdatedEventSubscriber = null;
	private bool _show = false;

	#region << Lifecycle >>

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		if (_pageOutdatedEventSubscriber is not null)
			await _pageOutdatedEventSubscriber.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		Navigator.LocationChanged += On_NavigationStateChanged;
		_pageOutdatedEventSubscriber = await TfEventBus.SubscribeAsync<TfPageOutdatedAlertEventPayload>(
			handler: On_ScreenAlertEventAsync,
			matchKey: (key) => key != TfAuthLayout.GetSessionId().ToString());
	}

	#endregion

	#region << Event Listeners >>

	private async Task On_ScreenAlertEventAsync(string? key, TfPageOutdatedAlertEventPayload? payload)
	{
		_show = true;
		await InvokeAsync(StateHasChanged);
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		_show = false;
		StateHasChanged();
	}

	#endregion

	private void _reload() => Navigator.ReloadCurrentUrl();
}