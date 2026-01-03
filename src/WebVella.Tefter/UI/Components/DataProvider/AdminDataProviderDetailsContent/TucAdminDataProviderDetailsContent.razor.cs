namespace WebVella.Tefter.UI.Components;

public partial class TucAdminDataProviderDetailsContent : TfBaseComponent, IAsyncDisposable
{
	private TfDataProvider? _provider = null;
	private TfDataProviderDisplaySettingsScreenRegion? _dynamicComponentContext = null;
	private TfScreenRegionScope? _dynamicComponentScope = null;
	private bool _isDeleting = false;
	private IAsyncDisposable _dataProviderUpdatedEventSubscriber = null!;

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		await _dataProviderUpdatedEventSubscriber.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		_dataProviderUpdatedEventSubscriber = await TfEventBus.SubscribeAsync<TfDataProviderUpdatedEventPayload>(
			handler: On_DataProviderUpdatedEventAsync,
			matchKey: (_) => true);
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
				await _init(TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task On_DataProviderUpdatedEventAsync(string? key, TfDataProviderUpdatedEventPayload? payload)
	{
		if(payload is null) return;
		if(payload.DataProvider.Id != _provider?.Id) return;
		if(key == TfAuthLayout.GetSessionId().ToString())
			await _init(TfAuthLayout.GetState().NavigationState);
		else
			await TfEventBus.PublishAsync(key: key, new TfPageOutdatedAlertEventPayload());
	}		

	private async Task _init(TfNavigationState navState)
	{
		try
		{
			if (navState.DataProviderId is null)
			{
				_provider = null;
				await InvokeAsync(StateHasChanged);
				return;
			}

			_provider = TfService.GetDataProvider(navState.DataProviderId.Value);
			if (_provider is null)
				return;
			_dynamicComponentContext = new TfDataProviderDisplaySettingsScreenRegion()
			{
				SettingsJson = _provider.SettingsJson
			};
			_dynamicComponentScope = new TfScreenRegionScope(_provider.ProviderType.GetType(), null);
		}
		finally
		{
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _editProvider()
	{
		if (_provider is null) return;
		var dialog = await DialogService.ShowDialogAsync<TucDataProviderManageDialog>(
			_provider,
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
		}
	}

	private async Task _deleteProvider()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm",
			    LOC("Are you sure that you need this data provider deleted?") + "\r\n" +
			    LOC("Will proceed only if there are not existing columns attached")))
			return;

		if (_provider is null) return;
		try
		{
			_isDeleting = true;
			await InvokeAsync(StateHasChanged);
			TfService.DeleteDataProvider(_provider.Id);
			ToastService.ShowSuccess(LOC("Data provider was successfully deleted"));
			await TfEventBus.PublishAsync(key:TfAuthLayout.GetSessionId().ToString(),new TfDataProviderDeletedEventPayload(_provider));
			_provider = null;
			Navigator.NavigateTo(TfConstants.AdminDataProvidersPageUrl, true);
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isDeleting = false;
			await InvokeAsync(StateHasChanged);
		}
	}
}