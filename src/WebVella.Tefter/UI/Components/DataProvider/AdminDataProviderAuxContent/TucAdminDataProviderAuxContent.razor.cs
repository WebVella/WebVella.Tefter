using ITfEventBus = WebVella.Tefter.UI.EventsBus.ITfEventBus;

namespace WebVella.Tefter.UI.Components;

public partial class TucAdminDataProviderAuxContent : TfBaseComponent, IAsyncDisposable
{
	[Inject] protected ITfEventBus TfEventBus { get; set; } = null!;
	private TfDataProvider? _provider = null;
	private List<TfDataProvider> _connectedProviders = new();
	
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
			handler: On_DataProviderUpdatedEventAsync);
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
		=> await _init(TfAuthLayout.GetState().NavigationState);

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
			_connectedProviders = TfService.GetDataProviderConnectedProviders(_provider.Id);
		}
		finally
		{
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _implementDataIdentity()
	{
		var dialog = await DialogService.ShowDialogAsync<TucDataProviderIdentityManageDialog>(
			new TfDataProviderIdentity() { DataProviderId = _provider!.Id },
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

	private async Task _editIdentity(TfDataProviderIdentity identity)
	{
		var dialog = await DialogService.ShowDialogAsync<TucDataProviderIdentityManageDialog>(
			identity,
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

	private async Task _deleteIdentity(TfDataProviderIdentity identity)
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this identity deleted?")))
			return;
		try
		{
			TfService.DeleteDataProviderIdentity(identity.Id);
			ToastService.ShowSuccess(LOC("The identity is successfully deleted!"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}

	private string _showCommonIdentities(TfDataProvider subProvider)
	{
		if (_provider is null) return "n/a";
		var commonIdentities = _provider.Identities
			.Select(x => x.DataIdentity)
			.Intersect(subProvider.Identities.Select(x => x.DataIdentity)).ToList();
		if (commonIdentities.Count == 0) return "n/a";
		return String.Join(", ", commonIdentities);
	}
}