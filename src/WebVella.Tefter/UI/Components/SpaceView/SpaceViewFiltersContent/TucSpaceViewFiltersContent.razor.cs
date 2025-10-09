// namespace WebVella.Tefter.UI.Components;
// public partial class TucSpaceViewFiltersContent : TfBaseComponent, IDisposable
// {
// 	private TfSpaceView _spaceView = null!;
// 	private TfDataset _spaceData = null!;
// 	private TfDataProvider _dataProvider = null!;
// 	private TfSpace _space = null!;
// 	public bool _submitting = false;
// 	public TfNavigationState? _navState = null;
// 	public void Dispose()
// 	{
// 		TfUIService.SpaceViewUpdated -= On_SpaceViewUpdated;
// 		Navigator.LocationChanged -= On_NavigationStateChanged;
// 	}
//
// 	protected override async Task OnInitializedAsync()
// 	{
// 		await _init(TfAuthLayout.AppState.NavigationState);
// 		TfUIService.SpaceViewUpdated += On_SpaceViewUpdated;
// 		Navigator.LocationChanged += On_NavigationStateChanged;
// 	}
//
// 	private async Task On_SpaceViewUpdated(object? caller, TfSpaceView args)
// 	{
// 		await _init(navState:TfAuthLayout.AppState.NavigationState, spaceView: args);
// 	}
//
// 	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
// 	{
// 		if (UriInitialized != args.Uri)
// 			await _init(navState: args.Payload);
// 	}
//
// 	private async Task _init(TfNavigationState navState, TfSpaceView? spaceView = null)
// 	{
// 		_navState = navState;
// 		try
// 		{
// 			if (spaceView is not null && spaceView.Id == _spaceView?.Id)
// 			{
// 				_spaceView = spaceView;
// 			}
// 			else
// 			{
// 				var routeData = TfAuthLayout.AppState.NavigationState;
// 				if (routeData.SpaceViewId is not null)
// 					_spaceView = TfUIService.GetSpaceView(routeData.SpaceViewId.Value);
//
// 			}
// 			if (_spaceView is null) return;
// 			_space = TfUIService.GetSpace(_spaceView.SpaceId);
// 			_spaceData = TfUIService.GetDataset(_spaceView.DatasetId);
// 			_dataProvider = TfUIService.GetDataProvider(_spaceData.DataProviderId);
// 		}
// 		finally
// 		{
// 			UriInitialized = _navState.Uri;
// 			await InvokeAsync(StateHasChanged);
// 		}
// 	}
//
// 	private async Task _onPresetsChanged(List<TfSpaceViewPreset> presets)
// 	{
// 		if (_submitting) return;
// 		try
// 		{
// 			TfUIService.UpdateSpaceViewPresets(
// 				viewId: _spaceView.Id,
// 				presets: presets);
// 		}
// 		catch (Exception ex)
// 		{
// 			ProcessException(ex);
// 		}
// 		finally
// 		{
// 			_submitting = false;
// 			await InvokeAsync(StateHasChanged);
// 		}
// 	}
// }