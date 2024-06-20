namespace WebVella.Tefter.Web.Components.SidebarToggle;
public partial class TfSidebarToggle : TfBaseComponent
{
	[Inject] protected IState<SessionState> SessionState { get; set; }
	private void _toggle()
	{
		Dispatcher.Dispatch(new SetUIAction(
		userId: SessionState.Value.UserId,
		spaceId: SessionState.Value.SpaceRouteId,
		spaceDataId: SessionState.Value.SpaceDataRouteId,
		spaceViewId: SessionState.Value.SpaceViewRouteId,
		mode:SessionState.Value.ThemeMode,
		color: SessionState.Value.ThemeColor,
		sidebarExpanded:!SessionState.Value.SidebarExpanded,
		cultureOption: SessionState.Value.CultureOption));
	}
}