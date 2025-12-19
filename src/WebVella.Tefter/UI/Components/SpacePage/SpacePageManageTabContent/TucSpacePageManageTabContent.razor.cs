namespace WebVella.Tefter.UI.Components;

public partial class TucSpacePageManageTabContent : TfBaseComponent, IAsyncDisposable
{
	[Inject] protected ITfEventBusEx TfEventBus { get; set; } = null!;
	private TfSpace _space = null!;
	private TfSpacePage _spacePage = null!;
	private IAsyncDisposable _spacePageUpdatedEventSubscriber = null!;

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		await _spacePageUpdatedEventSubscriber.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		_spacePageUpdatedEventSubscriber = await TfEventBus.SubscribeAsync<TfSpacePageUpdatedEventPayload>(
			handler: On_SpacePageUpdatedEventAsync);
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
				await _init(TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task On_SpacePageUpdatedEventAsync(string? key, TfSpacePageUpdatedEventPayload? payload)
		=> await _init(TfAuthLayout.GetState().NavigationState);

	private async Task _init(TfNavigationState navState)
	{
		try
		{
			if (navState.SpaceId is null)
				throw new Exception("Space Id not found in URL");
			_space = TfService.GetSpace(navState.SpaceId.Value) ?? throw new Exception("Space not found");
			if (navState.SpacePageId is null)
				throw new Exception("Page Id not found in URL");
			_spacePage = TfService.GetSpacePage(navState.SpacePageId.Value) ??
			             throw new Exception("Space page not found");
		}
		finally
		{
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private Dictionary<string, object> _getDynamicComponentParams()
	{
		var dict = new Dictionary<string, object> { ["Context"] = new TfSpacePageAddonContext
			{
				ComponentOptionsJson = _spacePage.ComponentOptionsJson,
				Icon = _spacePage.FluentIconName,
				Mode = TfComponentMode.Manage,
				SpacePage = _spacePage,
				Space = _space,
				CurrentUser = TfAuthLayout.GetState().User,
			}
		};
		return dict;
	}
}