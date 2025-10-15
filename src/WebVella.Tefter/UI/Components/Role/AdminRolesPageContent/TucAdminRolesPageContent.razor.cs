namespace WebVella.Tefter.UI.Components;
public partial class TucAdminRolesPageContent :TfBaseComponent,IDisposable
{
	[Inject] protected TfGlobalEventProvider TfEventProvider { get; set; } = null!;
	private bool _isLoading = false;
	private List<TfRole> _items = new();

	public void Dispose()
	{
		TfEventProvider.Dispose();
		Navigator.LocationChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		TfEventProvider.RoleCreatedEvent += On_RoleChanged;
		TfEventProvider.RoleUpdatedEvent += On_RoleChanged;
		Navigator.LocationChanged += On_NavigationStateChanged;
	}

	private async Task On_RoleChanged(object args)
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
			_items = TfService.GetRoles(navState.Search).ToList();
		}
		finally
		{
			_isLoading = false;
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}
	
	private async Task addRole()
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
		if (!result.Cancelled && result.Data != null) { }
	}		
}