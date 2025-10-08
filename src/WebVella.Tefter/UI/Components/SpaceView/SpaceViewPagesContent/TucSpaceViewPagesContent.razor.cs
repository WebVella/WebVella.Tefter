namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewPagesContent : TfBaseComponent, IDisposable
{
	private TfSpaceView _spaceView = new();
	private TfSpace _space = new();
	private TfNavigationState? _navState = null;
	private List<TfSpacePage> _items = new();

	public void Dispose()
	{
		TfState.NavigationStateChangedEvent -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfState.NavigationState);
		TfState.NavigationStateChangedEvent += On_NavigationStateChanged;
	}

	private async Task On_SpaceViewUpdated(object? caller, TfSpaceView args)
	{
		await InvokeAsync(async () =>
		{
			await _init(navState: TfState.NavigationState, spaceView: args);
		});
	}

	private async Task On_NavigationStateChanged(TfNavigationState args)
	{
		await InvokeAsync(async () =>
		{
			if (UriInitialized != args.Uri)
				await _init(navState: args);
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
				var routeData = TfState.NavigationState;
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