namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.Location.TfLocation", "WebVella.Tefter")]
public partial class TfLocation : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	private int _ellipsisCount = 30;

	private List<TucMenuItem> generateLocations()
	{
		var result = new List<TucMenuItem>();
		if (TfAppState.Value.Route is null) return result;
		if (TfAppState.Value.Route.HasNode(RouteDataNode.Home, 0))
		{
			result.Add(new TucMenuItem
			{
				Text = TfConverters.StringOverflow("Home", _ellipsisCount),
				Url = "/"
			});
		}
		if (TfAppState.Value.Route.HasNode(RouteDataNode.Pages, 0))
		{
			result.Add(new TucMenuItem
			{
				Text = TfConverters.StringOverflow("Pages", _ellipsisCount),
				Url = string.Format(TfConstants.PagesPageUrl)
			});
		}
		else if (TfAppState.Value.Route.HasNode(RouteDataNode.Admin, 0))
		{
			if (TfAppState.Value.Route.RouteNodes.Count > 1)
			{
				result.Add(new TucMenuItem
				{
					Text = TfConverters.StringOverflow(TfAppState.Value.Route.RouteNodes[1].ToDescriptionString(), _ellipsisCount)
				});
			}
			else{ 
				result.Add(new TucMenuItem
				{
					Text = TfConverters.StringOverflow("Dashboard", _ellipsisCount)
				});
			}
		}
		else if (TfAppState.Value.Route.HasNode(RouteDataNode.Space, 0))
		{
			if (TfAppState.Value.Space is not null)
			{
				result.Add(new TucMenuItem
				{
					Text = TfAppState.Value.Space.Name,
					Url = string.Format(TfConstants.SpacePageUrl, TfAppState.Value.Space.Id)
				});
				//if (TfAppState.Value.SpaceView is not null)
				//{
				//	result.Add(new TucMenuItem
				//	{
				//		Title = TfAppState.Value.SpaceView.Name,
				//		Url = String.Format(TfConstants.SpaceViewPageUrl, TfAppState.Value.Space.Id, TfAppState.Value.SpaceView.Id)
				//	});
				//}
				//else if (TfAppState.Value.SpaceData is not null)
				//{
				//	result.Add(new TucMenuItem
				//	{
				//		Title = TfAppState.Value.SpaceData.Name,
				//		Url = String.Format(TfConstants.SpaceDataPageUrl, TfAppState.Value.Space.Id, TfAppState.Value.SpaceData.Id)
				//	});
				//}
			}

		}


		return result;
	}
}