namespace WebVella.Tefter.UI.Components;

public partial class TucAdminDataProvidersPageContent : TfBaseComponent, IAsyncDisposable
{
	[Inject] protected ITfEventBusEx TfEventBus { get; set; } = null!;
	private bool _isLoading = false;
	private List<TfDataProvider> _items = new();
	private IAsyncDisposable _dataIdentityUpdatedEventSubscriber = null!;
	private IAsyncDisposable _providerCreatedEventSubscriber = null!;			

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		await _dataIdentityUpdatedEventSubscriber.DisposeAsync();		
		await _providerCreatedEventSubscriber.DisposeAsync();		
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		_dataIdentityUpdatedEventSubscriber = await TfEventBus.SubscribeAsync<TfDataIdentityUpdatedEventPayload>(
			handler: On_DataIdentityUpdatedEventAsync,
			key: TfAuthLayout.GetState().User.Id);
		_providerCreatedEventSubscriber = await TfEventBus.SubscribeAsync<TfDataProviderCreatedEventPayload>(
			handler:On_DataProviderCreatedEventAsync);		
	}

	private async Task On_DataIdentityUpdatedEventAsync(string? key, TfDataIdentityUpdatedEventPayload? payload)
		=> await _init(TfAuthLayout.GetState().NavigationState);
	private async Task On_DataProviderCreatedEventAsync(string? key, TfDataProviderCreatedEventPayload? payload)
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
			_items = TfService.GetDataProviders(navState.Search).ToList();
		}
		finally
		{
			_isLoading = false;
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}


	private async Task _addItem()
	{
		var dialog = await DialogService.ShowDialogAsync<TucDataProviderManageDialog>(
			new TfDataProvider(),
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