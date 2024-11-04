namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.Location.TfLocation", "WebVella.Tefter")]
public partial class TfLocation : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	private int _ellipsisCount = 30;

	private List<TucMenuItem> generateLocations()
	{
		var result = new List<TucMenuItem>();
		if (TfAppState.Value.Route.FirstNode == RouteDataFirstNode.Home)
		{
			result.Add(new TucMenuItem
			{
				Text = TfConverters.StringOverflow("Home", _ellipsisCount),
				Url = "/"
			});
		}
		if (TfAppState.Value.Route.FirstNode == RouteDataFirstNode.Pages)
		{
			result.Add(new TucMenuItem
			{
				Text = TfConverters.StringOverflow("Pages", _ellipsisCount),
				Url = String.Format(TfConstants.PagesPageUrl)
			});
		}
		else if (TfAppState.Value.Route.FirstNode == RouteDataFirstNode.Admin)
		{
			result.Add(new TucMenuItem
			{
				Text = TfConverters.StringOverflow(TfAppState.Value.Route.SecondNode.ToDescriptionString(), _ellipsisCount)
			});
		}
		else if (TfAppState.Value.Route.FirstNode == RouteDataFirstNode.Space)
		{
			if (TfAppState.Value.Space is not null)
			{
				result.Add(new TucMenuItem
				{
					Text = TfAppState.Value.Space.Name,
					Url = String.Format(TfConstants.SpacePageUrl, TfAppState.Value.Space.Id)
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