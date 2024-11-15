namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceData.SpaceDataDetailsNav.TfSpaceDataDetailsNav", "WebVella.Tefter")]
public partial class TfSpaceDataDetailsNav : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }

	private List<TucSpaceView> viewList = new();


	private List<TucMenuItem> _generateMenu()
	{
		var menu = new List<TucMenuItem>();
		if(TfAppState.Value.Route is null || TfAppState.Value.Route.DataProviderId is null)
			return menu;
		var providerId = TfAppState.Value.Route.DataProviderId.Value;
		menu.Add(new TucMenuItem
		{
			Url = String.Format(TfConstants.SpaceDataPageUrl, TfAppState.Value.Route.SpaceId, TfAppState.Value.Route.SpaceDataId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Info(),
			Text = LOC("Details")
		});
		menu.Add(new TucMenuItem
		{
			Url = String.Format(TfConstants.SpaceDataViewsPageUrl, TfAppState.Value.Route.SpaceId, TfAppState.Value.Route.SpaceDataId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Table(),
			Text = LOC("Used in Views")
		});
		menu.Add(new TucMenuItem
		{
			Url = String.Format(TfConstants.SpaceDataDataPageUrl, TfAppState.Value.Route.SpaceId, TfAppState.Value.Route.SpaceDataId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Table(),
			Text = LOC("Data")
		});
		return menu;
	}

	
}