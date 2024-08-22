namespace WebVella.Tefter.Web.Layout;
public partial class SpaceLayout : FluxorLayout
{
	[Inject] protected IState<ThemeState> ThemeState { get; set; }
	[Inject] protected IState<SpaceState> SpaceState { get; set; }
	[Inject] protected IStateSelection<ScreenState, bool> ScreenStateSidebarExpanded { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);
	}
}