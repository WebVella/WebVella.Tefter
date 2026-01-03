namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceHeader : TfBaseComponent, IAsyncDisposable
{
	private TfSpace? _space = null!;
	private IAsyncDisposable _spaceUpdatedEventSubscriber = null!;	
	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		await _spaceUpdatedEventSubscriber.DisposeAsync();
	}
	protected override async Task OnInitializedAsync()
	{
		_space = TfAuthLayout.GetState().Space;
		Navigator.LocationChanged += On_NavigationStateChanged;
		_spaceUpdatedEventSubscriber = await TfEventBus.SubscribeAsync<TfSpaceUpdatedEventPayload>(
			handler:On_SpaceUpdatedEventAsync,
			matchKey: (_) => true);
	}
	
	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		_space = TfAuthLayout.GetState().Space;
		StateHasChanged();
	}

	private async Task On_SpaceUpdatedEventAsync(string? key, TfSpaceUpdatedEventPayload? payload)
	{
		if(payload is null) return;
		if(payload.Space.Id != payload?.Space?.Id) return;
		if (key == TfAuthLayout.GetSessionId().ToString())
		{
			if(_space is null) return;
			_space = payload!.Space;
			await InvokeAsync(StateHasChanged);
		}
		else
			await TfEventBus.PublishAsync(key: key, new TfPageOutdatedAlertEventPayload());
	}		

	private void _manageCurrentSpace()
	{
		var pageManageUrl = String.Format(TfConstants.SpaceManagePageUrl, TfAuthLayout.GetState().Space!.Id);
		Navigator.NavigateTo(pageManageUrl.GenerateWithLocalAndQueryAsReturnUrl(Navigator.Uri)!);
	}


	private async Task _deleteSpace()
	{
		var state = TfAuthLayout.GetState();
		if (state.Space is null) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this space deleted?")))
			return;
		try
		{
			TfService.DeleteSpace(state.Space.Id);
			ToastService.ShowSuccess(LOC("Space deleted"));
			await TfEventBus.PublishAsync(key:TfAuthLayout.GetSessionId(), 
				payload: new TfSpaceDeletedEventPayload(state.Space));				
			Navigator.NavigateTo(TfConstants.HomePageUrl);
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}
}
