namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceManageDetailsContent : TfBaseComponent, IAsyncDisposable
{
	[Inject] protected ITfEventBusEx TfEventBus { get; set; } = null!;
	private TfSpace? _space = null;
	private TfNavigationState _navState = null!;
	private IAsyncDisposable _spaceEventSubscriber = null!;

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		await _spaceEventSubscriber.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		_spaceEventSubscriber = await TfEventBus.SubscribeAsync<TfSpaceEventPayload>(
			handler: On_SpaceEventAsync);
	}


	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
				await _init(navState: TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task On_SpaceEventAsync(string? key, TfSpaceEventPayload? payload)
		=> await _init(TfAuthLayout.GetState().NavigationState);


	private async Task _init(TfNavigationState navState)
	{
		_navState = navState;

		try
		{
			if (_navState.SpaceId is null) return;
			_space = TfService.GetSpace(_navState.SpaceId.Value);
			if (_space is null) return;
		}
		finally
		{
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _editSpace()
	{
		if(_space is null) return;
		var dialog = await DialogService.ShowDialogAsync<TucSpaceManageDialog>(
			_space,
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

	private async Task _deleteSpace()
	{
		if(_space is null) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this space deleted?")))
			return;
		try
		{
			TfService.DeleteSpace(_space.Id);
			ToastService.ShowSuccess(LOC("Space deleted"));
			Navigator.NavigateTo(TfConstants.HomePageUrl);
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}
}