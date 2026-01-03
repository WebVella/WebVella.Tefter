namespace WebVella.Tefter.UI.Components;
public partial class TucAdminRolesPageContent :TfBaseComponent,IAsyncDisposable
{
	private bool _isLoading = false;
	private List<TfRole> _items = new();
	private IAsyncDisposable _roleEventSubscriber = null!;
	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		await _roleEventSubscriber.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		_roleEventSubscriber = await TfEventBus.SubscribeAsync<TfRoleEventPayload>(
			handler: On_RoleEventAsync);		
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
	private async Task On_RoleEventAsync(string? key, TfRoleEventPayload? payload)
		=> await _init(TfAuthLayout.GetState().NavigationState);
	private async Task _init(TfNavigationState navState)
	{
		try
		{
			_items = TfService.GetRoles(navState.Search).ToList();
		}
		finally
		{
			_isLoading = false;
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}
	
	private async Task _addRole()
	{
		var dialog = await DialogService.ShowDialogAsync<TucRoleManageDialog>(
			new TfRole(),
			new ()
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