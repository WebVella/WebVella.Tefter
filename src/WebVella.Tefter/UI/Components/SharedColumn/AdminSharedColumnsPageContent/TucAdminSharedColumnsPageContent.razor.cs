namespace WebVella.Tefter.UI.Components;

public partial class TucAdminSharedColumnsPageContent : TfBaseComponent, IAsyncDisposable
{
	private bool _isLoading = false;
	private List<TfSharedColumn> _items = new();
	private IAsyncDisposable? _sharedColumnEventSubscriber = null;

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		if (_sharedColumnEventSubscriber is not null)
			await _sharedColumnEventSubscriber.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		_sharedColumnEventSubscriber = await TfEventBus.SubscribeAsync<TfSharedColumnEventPayload>(
			handler: On_SharedColumnEventAsync,
			matchKey: (_) => true);
	}

	private async Task On_SharedColumnEventAsync(string? key, TfSharedColumnEventPayload? payload)
	{
		if (payload is null) return;
		if (key == TfAuthLayout.GetSessionId().ToString())
			await _init(TfAuthLayout.GetState().NavigationState);
		else
			await TfEventBus.PublishAsync(key: key, new TfPageOutdatedAlertEventPayload());
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
			{
				await _init(TfAuthLayout.GetState().NavigationState);
			}
		});
	}

	private async Task _init(TfNavigationState navState)
	{
		try
		{
			_items = TfService.GetSharedColumns(navState.Search).ToList();
		}
		finally
		{
			_isLoading = false;
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _onAddClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TucSharedColumnManageDialog>(
			new TfSharedColumn(),
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (result is { Cancelled: false, Data: not null }) { }
	}
}