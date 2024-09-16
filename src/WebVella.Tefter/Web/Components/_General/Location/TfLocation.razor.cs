namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Location.TfLocation","WebVella.Tefter")]
public partial class TfLocation : TfBaseComponent
{
	[Inject] protected IState<TfRouteState> TfRouteState { get; set; }
	private int _ellipsisCount = 30;

	private List<MenuItem> generateLocations()
	{
		var result = new List<MenuItem>();
		if(TfRouteState.Value.FirstNode == RouteDataFirstNode.Home){ 
			result.Add(new MenuItem{ 
				Title = "Home",
				Url = "/"
			});		
		}
		else if(TfRouteState.Value.FirstNode == RouteDataFirstNode.Admin){ 
			result.Add(new MenuItem{ 
				Title = "Administration",
				Url = String.Format(TfConstants.AdminPageUrl)
			});
		}
		else if(TfRouteState.Value.FirstNode == RouteDataFirstNode.FastAccess){ 
			result.Add(new MenuItem{ 
				Title = "Fast Access",
				Url = String.Format(TfConstants.FastAccessPageUrl)
			});		
		}
		else if(TfRouteState.Value.FirstNode == RouteDataFirstNode.Space){ 
			result.Add(new MenuItem{ 
				Title = "Space",
				Url = String.Format(TfConstants.SpacePageUrl,TfRouteState.Value.SpaceId)
			});		
		}


		return result;
	}
}