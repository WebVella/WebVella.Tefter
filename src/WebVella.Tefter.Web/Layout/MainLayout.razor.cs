using WebVella.Tefter.Web.Store.SessionState;
using WebVella.Tefter.Web.Store.UserState;

namespace WebVella.Tefter.Web.Layout;
public partial class MainLayout : FluxorLayout
{
	[Inject] protected IState<ThemeState> ThemeState { get; set; }
	[Inject] protected IStateSelection<ScreenState, bool> ScreenStateSidebarExpanded { get; set; }
	[Inject] protected IStateSelection<SessionState,string> SessionSpaceColor { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);
		SessionSpaceColor.Select(x=> x.SpaceColor);
	}
}