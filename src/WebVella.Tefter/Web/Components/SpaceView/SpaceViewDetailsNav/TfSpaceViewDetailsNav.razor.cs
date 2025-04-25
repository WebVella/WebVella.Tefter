namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewDetailsNav.TfSpaceViewDetailsNav", "WebVella.Tefter")]
public partial class TfSpaceViewDetailsNav : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }

	private List<TucSpaceView> viewList = new();


	private List<TucMenuItem> _generateMenu()
	{
		var menu = new List<TucMenuItem>();
		if (TfAppState.Value.Route is null) return menu;
		menu.Add(new TucMenuItem
		{
			Url = string.Format(TfConstants.SpaceViewPageUrl, TfAppState.Value.Route.SpaceId, TfAppState.Value.Route.SpaceViewId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Info(),
			Text = LOC("Details")
		});
		menu.Add(new TucMenuItem
		{
			Url = string.Format(TfConstants.SpaceViewPagesPageUrl, TfAppState.Value.Route.SpaceId, TfAppState.Value.Route.SpaceViewId),
			Match = NavLinkMatch.Prefix,
			//Icon = new Icons.Regular.Size20.Table(),
			Text = LOC("Connected pages")
		});
		return menu;
	}


}