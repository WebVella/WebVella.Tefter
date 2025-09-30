namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewFiltersContent : TfBaseComponent, IDisposable
{
	private TfSpaceView _spaceView = default!;
	private TfDataset _spaceData = default!;
	private TfDataProvider _dataProvider = default!;
	private TfSpace _space = default!;
	public bool _submitting = false;
	public TfNavigationState? _navState = null;
	public void Dispose()
	{
		TfUIService.SpaceViewUpdated -= On_SpaceViewUpdated;
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfUIService.SpaceViewUpdated += On_SpaceViewUpdated;
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
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
			_navState = TfAuthLayout.NavigationState;
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
					_spaceView = TfUIService.GetSpaceView(routeData.SpaceViewId.Value);

			}
			if (_spaceView is null) return;
			_space = TfUIService.GetSpace(_spaceView.SpaceId);
			_spaceData = TfUIService.GetDataset(_spaceView.DatasetId);
			_dataProvider = TfUIService.GetDataProvider(_spaceData.DataProviderId);
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
			TfUIService.UpdateSpaceViewPresets(
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