namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceDetailsNav.TfSpaceDetailsNav", "WebVella.Tefter")]
public partial class TfSpaceDetailsNav : TfBaseComponent
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
			Url = string.Format(TfConstants.SpaceManagePageUrl, TfAppState.Value.Route.SpaceId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Info(),
			Text = LOC("Details")
		});
		menu.Add(new TucMenuItem
		{
			Url = string.Format(TfConstants.SpaceManagePagesPageUrl, TfAppState.Value.Route.SpaceId),
			Match = NavLinkMatch.Prefix,
			//Icon = new Icons.Regular.Size20.Table(),
			Text = LOC("Pages")
		});
		menu.Add(new TucMenuItem
		{
			Url = string.Format(TfConstants.SpaceManageAccessPageUrl, TfAppState.Value.Route.SpaceId),
			Match = NavLinkMatch.Prefix,
			//Icon = new Icons.Regular.Size20.Table(),
			Text = LOC("Access")
		});
		return menu;
	}


}