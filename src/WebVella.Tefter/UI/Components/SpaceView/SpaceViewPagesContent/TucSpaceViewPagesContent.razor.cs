namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewPagesContent : TfBaseComponent, IDisposable
{
	private TfSpaceView _spaceView = new();
	private TfSpace _space = new();
	private TfNavigationState? _navState = null;
	private List<TfSpacePage> _items = new();

	public void Dispose()
	{
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_SpaceViewUpdated(object? caller, TfSpaceView args)
	{
		await _init(spaceView: args);
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
			_items = (TfUIService.GetSpacePages(_space.Id) ?? new List<TfSpacePage>())
				.Where(x => x.ComponentOptionsJson.Contains(_spaceView.Id.ToString())).ToList();;

		}
		finally
		{
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	

}