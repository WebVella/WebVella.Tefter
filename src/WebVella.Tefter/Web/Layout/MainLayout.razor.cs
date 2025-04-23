namespace WebVella.Tefter.Web.Layout;
public partial class MainLayout : FluxorLayout
{
	[Inject] protected IStateSelection<TfUserState, bool> SidebarExpanded { get; set; }
	[Inject] protected IStateSelection<TfUserState, DesignThemeModes> ThemeMode { get; set; }
	[Inject] protected IStateSelection<TfUserState, TfColor> ThemeColor { get; set; }
	[Inject] protected IStateSelection<TfAppState, RouteDataFirstNode?> FirstRouteNode { get; set; }
	[Inject] protected IStateSelection<TfAppState, TfColor> SpaceColor { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
		SidebarExpanded.Select(x => x.SidebarExpanded);
		ThemeMode.Select(x => x.ThemeMode);
		ThemeColor.Select(x => x.ThemeColor);
		FirstRouteNode.Select(x=> x.Route.FirstNode);
		SpaceColor.Select(x=> x.SpaceColor);
	}

	private TfColor _layoutColor
	{
		get
		{
			if(FirstRouteNode.Value is not null 
				&& FirstRouteNode.Value == RouteDataFirstNode.Space){ 
				return SpaceColor.Value;
			}

			return ThemeColor.Value;
		}
	}
}