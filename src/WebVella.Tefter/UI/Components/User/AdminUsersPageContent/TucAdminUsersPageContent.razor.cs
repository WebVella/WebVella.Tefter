namespace WebVella.Tefter.UI.Components;

public partial class TucAdminUsersPageContent : TfBaseComponent, IAsyncDisposable
{
	private bool _isLoading = false;
	private List<TfUser> _items = new();
	private IAsyncDisposable _userEventSubscriber = null!;

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		await _userEventSubscriber.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		_userEventSubscriber = await TfEventBus.SubscribeAsync<TfUserEventPayload>(
			handler: On_UserEventAsync,
			matchKey: (_) => true);
	}

	private async Task On_UserEventAsync(string? key, TfUserEventPayload? payload)
	{
		if(payload is null) return;
		if(key == TfAuthLayout.GetSessionId().ToString())
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
			_items = TfService.GetUsers(navState.Search).ToList();
		}
		finally
		{
			_isLoading = false;
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task addItem()
	{
		var dialog = await DialogService.ShowDialogAsync<TucUserManageDialog>(
			new TfUser(),
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null) { }
	}
}