using WebVella.Tefter.UI.Pages;

namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewPageContentToolbar : TfBaseComponent, IAsyncDisposable
{
	// Dependency Injection
	[CascadingParameter(Name = "TucSpaceViewPageContent")]
	public TucSpaceViewPageContent TucSpaceViewPageContent { get; set; } = null!;

	[Parameter] public TfSpacePageAddonContext Context { get; set; } = null!;
	[Parameter] public TfSpaceView SpaceView { get; set; } = null!;
	[Parameter] public TfDataset Dataset { get; set; } = null!;
	[Parameter] public TfSpaceViewPreset? SpaceViewPreset { get; set; } = null;
	[Parameter] public TfDataTable Data { get; set; } = null!;
	[Parameter] public EventCallback<TfDataTable> DataChanged { get; set; }
	[Parameter] public List<Guid> SelectedRows { get; set; } = new();

	private TfNavigationState _navState = null!;
	private bool _hasViewPersonalization = false;
	private Guid _initedSpaceViewId = Guid.Empty;
	private bool _hasPinnedData = false;

	private IAsyncDisposable? _userUpdatedEventSubscriber = null;

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		if (_userUpdatedEventSubscriber is not null)
			await _userUpdatedEventSubscriber.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_navState = TfAuthLayout.GetState().NavigationState;
		await _init(_navState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		_userUpdatedEventSubscriber = await TfEventBus.SubscribeAsync<TfUserUpdatedEventPayload>(
			handler: On_UserUpdatedEventAsync);
	}

	protected override async Task OnParametersSetAsync()
	{
		if (_initedSpaceViewId != SpaceView.Id)
			await _init(TfAuthLayout.GetState().NavigationState);
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
				await _init(TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task On_UserUpdatedEventAsync(string? key, TfUserUpdatedEventPayload? payload)
	{
		if (payload is null) return;
		Context.CurrentUser = payload.User;
		await _init(TfAuthLayout.GetState().NavigationState);
	}

	private async Task _init(TfNavigationState navState)
	{
		_navState = navState;
		try
		{
			string? pinnedDataIdentity = Navigator.GetStringFromQuery(TfConstants.DataIdentityIdQueryName);
			string? pinnedDataIdentityValue =
				Navigator.GetStringFromQuery(TfConstants.DataIdentityValueQueryName);
			_hasPinnedData = !string.IsNullOrWhiteSpace(pinnedDataIdentity) &&
			                 !string.IsNullOrWhiteSpace(pinnedDataIdentityValue);

			_hasViewPersonalization = Context.CurrentUser.Settings.ViewPresetColumnPersonalizations.Any(x =>
				x.SpaceViewId == SpaceView.Id && x.PresetId == SpaceViewPreset?.Id);
			if (!_hasViewPersonalization)
			{
				_hasViewPersonalization = Context.CurrentUser.Settings.ViewPresetSortPersonalizations.Any(x =>
					x.SpaceViewId == SpaceView.Id
					&& x.PresetId == SpaceViewPreset?.Id);
			}
		}
		finally
		{
			UriInitialized = _navState.Uri;
			_initedSpaceViewId = SpaceView.Id;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _searchChanged(string value) => await TucSpaceViewPageContent.OnSearch(value);

	private async Task _onAddRowClick()
	{
		try
		{
			var result = TfService.InsertRowInDataTable(Data);
			TucSpaceViewPageContent.OnNewRow(result);
			ToastService.ShowSuccess(LOC("Row added"));
			var provider = TfService.GetDataProvider(result.QueryInfo.DataProviderId);
			await TfEventBus.PublishAsync(TfAuthLayout.GetSessionId().ToString(),
				new TfDataProviderDataChangedEventPayload(provider!));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}

	private void _onEditAllClick()
	{
		TucSpaceViewPageContent.ToggleEditAll();
	}

	private TfSpaceViewToolBarActionScreenRegion _getToolbarActionsContext()
	{
		return new TfSpaceViewToolBarActionScreenRegion()
		{
			CurrentUser = Context.CurrentUser,
			Data = Data,
			SpaceView = SpaceView,
			Dataset = Dataset,
			SelectedDataRows =  SelectedRows,
			SpacePage =  Context.SpacePage,
			TucSpaceViewPageContent =  TucSpaceViewPageContent
		};
	}
}