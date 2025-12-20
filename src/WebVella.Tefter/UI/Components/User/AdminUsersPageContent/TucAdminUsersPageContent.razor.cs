namespace WebVella.Tefter.UI.Components;

public partial class TucAdminUsersPageContent : TfBaseComponent, IAsyncDisposable
{
	[Inject] protected ITfEventBusEx TfEventBus { get; set; } = null!;
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
			handler: On_UserEventAsync);
	}

	private async Task On_UserEventAsync(string? key, TfUserEventPayload? payload)
		=> await _init(TfAuthLayout.GetState().NavigationState);

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