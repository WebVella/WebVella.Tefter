namespace WebVella.Tefter.Web.Layout;
public partial class MainLayout : FluxorLayout
{
	[Inject] protected IState<TfUserState> UserState { get; set; }
	[Inject] protected IStateSelection<TfUserState, bool> SidebarExpanded { get; set; }
	[Inject] protected IStateSelection<TfUserState, DesignThemeModes> ThemeMode { get; set; }
	[Inject] protected IStateSelection<TfUserState, TfColor> ThemeColor { get; set; }
	[Inject] protected IStateSelection<TfAppState, RouteDataNode?> FirstRouteNode { get; set; }
	[Inject] protected IStateSelection<TfAppState, TfColor> SpaceColor { get; set; }
	[Inject] protected NavigationManager Navigator { get; set; }
	[Inject] public IActionSubscriber ActionSubscriber { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
			Navigator.LocationChanged -= Navigator_LocationChanged;
		}
		await base.DisposeAsyncCore(disposing);
	}


	protected override void OnInitialized()
	{
		base.OnInitialized();
		_checkAccess();
		SidebarExpanded.Select(x => x.SidebarExpanded);
		ThemeMode.Select(x => x.ThemeMode);
		ThemeColor.Select(x => x.ThemeColor);
		FirstRouteNode.Select(x => x.Route.RouteNodes[0]);
		SpaceColor.Select(x => x.SpaceColor);
	}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			Navigator.LocationChanged += Navigator_LocationChanged;
			ActionSubscriber.SubscribeToAction<SetAppStateAction>(this, On_StateChanged);
		}
	}

	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
		_checkAccess();
	}

	private void On_StateChanged(SetAppStateAction action)
	{
		_checkAccess();
	}

	private TfColor _layoutColor
	{
		get
		{
			if (FirstRouteNode.Value is not null
				&& FirstRouteNode.Value == RouteDataNode.Space)
			{
				return SpaceColor.Value;
			}

			return ThemeColor.Value;
		}
	}

	private void _checkAccess()
	{
		if (UC.UserHasAccess(UserState.Value.CurrentUser, Navigator)) return;
		Navigator.NavigateTo(string.Format(TfConstants.NoAccessPage));
	}
}