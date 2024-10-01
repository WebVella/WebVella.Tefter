namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.Location.TfLocation", "WebVella.Tefter")]
public partial class TfLocation : TfBaseComponent
{
	[Inject] protected IState<TfRouteState> TfRouteState { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	private int _ellipsisCount = 30;

	private List<TucMenuItem> generateLocations()
	{
		var result = new List<TucMenuItem>();
		if (TfRouteState.Value.FirstNode == RouteDataFirstNode.Home)
		{
			result.Add(new TucMenuItem
			{
				Title = TfConverters.StringOverflow("Home", _ellipsisCount),
				Url = "/"
			});
		}
		if (TfRouteState.Value.FirstNode == RouteDataFirstNode.Pages)
		{
			result.Add(new TucMenuItem
			{
				Title = TfConverters.StringOverflow("Pages", _ellipsisCount),
				Url = String.Format(TfConstants.PagesPageUrl)
			});
		}
		else if (TfRouteState.Value.FirstNode == RouteDataFirstNode.Admin)
		{
			result.Add(new TucMenuItem
			{
				Title = TfConverters.StringOverflow("Administration", _ellipsisCount),
				Url = String.Format(TfConstants.AdminDashboardUrl)
			});
		}
		else if (TfRouteState.Value.FirstNode == RouteDataFirstNode.Space)
		{
			if (TfAppState.Value.Space is not null)
			{
				result.Add(new TucMenuItem
				{
					Title = TfAppState.Value.Space.Name,
					Url = String.Format(TfConstants.SpacePageUrl, TfAppState.Value.Space.Id)
				});
				if (TfAppState.Value.SpaceView is not null)
				{
					result.Add(new TucMenuItem
					{
						Title = TfAppState.Value.SpaceView.Name,
						Url = String.Format(TfConstants.SpaceViewPageUrl, TfAppState.Value.Space.Id, TfAppState.Value.SpaceView.Id)
					});
				}
				else if (TfAppState.Value.SpaceData is not null)
				{
					result.Add(new TucMenuItem
					{
						Title = TfAppState.Value.SpaceData.Name,
						Url = String.Format(TfConstants.SpaceDataPageUrl, TfAppState.Value.Space.Id, TfAppState.Value.SpaceData.Id)
					});
				}
			}

		}


		return result;
	}
}