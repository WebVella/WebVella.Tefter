namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewPagesContent : TfBaseComponent, IDisposable
{
	private TfSpaceView _spaceView = new();
	private TfSpace _space = new();
	private TfNavigationState? _navState = null;
	private List<TfSpacePage> _items = new();

	public void Dispose()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
	}

	private async Task On_SpaceViewUpdated(object? caller, TfSpaceView args)
	{
		await InvokeAsync(async () =>
		{
			await _init(navState: TfAuthLayout.GetState().NavigationState, spaceView: args);
		});
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
				await _init(navState: TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task _init(TfNavigationState navState, TfSpaceView? spaceView = null)
	{
		_navState = navState;
		try
		{
			if (spaceView is not null && spaceView.Id == _spaceView?.Id)
			{
				_spaceView = spaceView;
			}
			else
			{
				var routeData = TfAuthLayout.GetState().NavigationState;
				if (routeData.SpaceViewId is not null)
					_spaceView = TfService.GetSpaceView(routeData.SpaceViewId.Value);

			}
			if (_spaceView is null) return;
			_space = TfService.GetSpace(_spaceView.SpaceId);
			_items = (TfService.GetSpacePages(_space.Id) ?? new List<TfSpacePage>())
				.Where(x => x.ComponentOptionsJson.Contains(_spaceView.Id.ToString())).ToList();;

		}
		finally
		{
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	

}