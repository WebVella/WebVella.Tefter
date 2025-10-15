namespace WebVella.Tefter.UI.Components;

public partial class TucAdminDataProvidersPageContent : TfBaseComponent,IDisposable
{
	[Inject] protected TfGlobalEventProvider TfEventProvider { get; set; } = null!;
	private bool _isLoading = false;
	private List<TfDataProvider> _items = new();

	public void Dispose()
	{
		TfEventProvider.Dispose();
		Navigator.LocationChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		TfEventProvider.DataProviderCreatedEvent += On_UserChanged;
		TfEventProvider.DataIdentityUpdatedEvent += On_UserChanged;
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
			_items = TfService.GetDataProviders(navState.Search).ToList();
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
		var dialog = await DialogService.ShowDialogAsync<TucDataProviderManageDialog>(
		new TfDataProvider(),
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