namespace WebVella.Tefter.UI.Components;

public partial class TucAdminDataIdentitiesPageContent : TfBaseComponent, IAsyncDisposable
{
	private bool _isLoading = false;
	private List<TfDataIdentity> _items = new();
	private IAsyncDisposable _dataIdentityEventSubscriber = null!;
	private IAsyncDisposable _providerCreatedEventSubscriber = null!;

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		await _dataIdentityEventSubscriber.DisposeAsync();
		await _providerCreatedEventSubscriber.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		_dataIdentityEventSubscriber = await TfEventBus.SubscribeAsync<TfDataIdentityEventPayload>(
			handler: On_DataIdentityEventAsync,
			matchKey: (_) => true);
		_providerCreatedEventSubscriber = await TfEventBus.SubscribeAsync<TfDataProviderEventPayload>(
			handler: On_DataProviderEventAsync,
			matchKey: (_) => true);
	}

	private async Task On_DataIdentityEventAsync(string? key, TfDataIdentityEventPayload? payload)
	{
		if(payload is null) return;
		if(key == TfAuthLayout.GetSessionId().ToString())
			await _init(TfAuthLayout.GetState().NavigationState);
		else
			await TfEventBus.PublishAsync(key: key, new TfPageOutdatedAlertEventPayload());
	}


	private async Task On_DataProviderEventAsync(string? key, TfDataProviderEventPayload? payload)
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
			_items = TfService.GetDataIdentities(navState.Search).ToList();
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
		var dialog = await DialogService.ShowDialogAsync<TucDataIdentityManageDialog>(
			new TfDataIdentity(),
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (result is { Cancelled: false, Data: not null })
		{
			var item = (TfDataIdentity)result.Data;
			Navigator.NavigateTo(string.Format(TfConstants.AdminDataIdentityDetailsPageUrl, item.DataIdentity));
		}
	}
}