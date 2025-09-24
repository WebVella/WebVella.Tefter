namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewFiltersContent : TfBaseComponent, IDisposable
{
	[Inject] public ITfSpaceViewUIService TfSpaceViewUIService { get; set; } = default!;
	[Inject] public ITfDatasetUIService TfDatasetUIService { get; set; } = default!;
	[Inject] public ITfDataProviderUIService TfDataProviderUIService { get; set; } = default!;
	[Inject] public ITfSpaceUIService TfSpaceUIService { get; set; } = default!;
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;

	private TfSpaceView _spaceView = default!;
	private TfDataset _spaceData = default!;
	private TfDataProvider _dataProvider = default!;
	private TfSpace _space = default!;
	public bool _submitting = false;
	public TfNavigationState? _navState = null;
	public void Dispose()
	{
		TfSpaceViewUIService.SpaceViewUpdated -= On_SpaceViewUpdated;
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfSpaceViewUIService.SpaceViewUpdated += On_SpaceViewUpdated;
		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_SpaceViewUpdated(object? caller, TfSpaceView args)
	{
		await _init(spaceView: args);
	}

	private async void On_SpaceViewColumnsUpdated(object? caller, List<TfSpaceViewColumn> args)
	{
		await _init(null);
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(navState: args);
	}

	private async Task _init(TfNavigationState? navState = null, TfSpaceView? spaceView = null)
	{
		if (navState == null)
			_navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		else
			_navState = navState;
		try
		{
			if (spaceView is not null && spaceView.Id == _spaceView?.Id)
			{
				_spaceView = spaceView;
			}
			else
			{
				var routeData = Navigator.GetRouteState();
				if (routeData.SpaceViewId is not null)
					_spaceView = TfSpaceViewUIService.GetSpaceView(routeData.SpaceViewId.Value);

			}
			if (_spaceView is null) return;
			_space = TfSpaceUIService.GetSpace(_spaceView.SpaceId);
			_spaceData = TfDatasetUIService.GetDataset(_spaceView.DatasetId);
			_dataProvider = TfDataProviderUIService.GetDataProvider(_spaceData.DataProviderId);
		}
		finally
		{
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _onPresetsChanged(List<TfSpaceViewPreset> presets)
	{
		if (_submitting) return;
		try
		{
			TfSpaceViewUIService.UpdateSpaceViewPresets(
				viewId: _spaceView.Id,
				presets: presets);
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_submitting = false;
			await InvokeAsync(StateHasChanged);
		}
	}
}