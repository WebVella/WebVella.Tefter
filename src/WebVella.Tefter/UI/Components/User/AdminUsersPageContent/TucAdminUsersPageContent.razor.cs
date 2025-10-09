namespace WebVella.Tefter.UI.Components;

public partial class TucAdminUsersPageContent : TfBaseComponent, IDisposable
{
	[Inject] protected TfGlobalEventProvider TfEventProvider { get; set; } = null!;
	private bool _isLoading = false;
	private List<TfUser> _items = new();

	public void Dispose()
	{
		TfEventProvider?.Dispose();
		Navigator.LocationChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		TfEventProvider.UserCreatedEvent += On_UserChanged;
		TfEventProvider.UserUpdatedEvent += On_UserChanged;
		Navigator.LocationChanged += On_NavigationStateChanged;
	}

	private async Task On_UserChanged(object args)
	{
		await InvokeAsync(async () =>
		{
			await _init(TfAuthLayout.GetState().NavigationState);
		});
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
			new DialogParameters()
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