namespace WebVella.Tefter.UI.Components;
public partial class TucAdminDataIdentityDetailsContent : TfBaseComponent, IAsyncDisposable
{
	private TfDataIdentity? _identity = null;
	private IAsyncDisposable? _dataIdentityUpdatedEventSubscriber = null;
	
	
	public async ValueTask DisposeAsync()
	{
		if(_dataIdentityUpdatedEventSubscriber is not null)
			await _dataIdentityUpdatedEventSubscriber.DisposeAsync();
		Navigator.LocationChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await _init(navState:TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		_dataIdentityUpdatedEventSubscriber = await TfEventBus.SubscribeAsync<TfDataIdentityUpdatedEventPayload>(
			handler:On_DataIdentityUpdatedEventAsync,
			matchKey: (_) => true);
	}

	private async Task On_DataIdentityUpdatedEventAsync(string? key, TfDataIdentityUpdatedEventPayload? payload)
	{
		if (payload is null) return;
		if(payload.DataIdentity.DataIdentity != _identity?.DataIdentity) return;
		if (key == TfAuthLayout.GetSessionId().ToString())
			await _init(navState: TfAuthLayout.GetState().NavigationState, identity: payload.DataIdentity);
		else
			await TfEventBus.PublishAsync(key: key, new TfPageOutdatedAlertEventPayload());


	}		

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
				await _init(navState: TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task _init(TfNavigationState navState, TfDataIdentity? identity = null)
	{
		try
		{
			if (identity is not null && identity.DataIdentity == _identity?.DataIdentity)
			{
				_identity = identity;
			}
			else
			{
				if (navState.DataIdentityId is not null)
					_identity = TfService.GetDataIdentity(navState.DataIdentityId);
			}
		}
		finally
		{
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _editIdentity()
	{
		if(_identity is null) return;
		var dialog = await DialogService.ShowDialogAsync<TucDataIdentityManageDialog>(
		_identity,
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

	private async Task _deleteIdentity()
	{
		if(_identity is null) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this identity deleted?")))
			return;
		try
		{
			TfService.DeleteDataIdentity(_identity.DataIdentity);
			ToastService.ShowSuccess(LOC("Data identity removed"));
			await TfEventBus.PublishAsync(key:TfAuthLayout.GetSessionId(), 
				payload: new TfDataIdentityDeletedEventPayload(_identity));			
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}

}