

namespace WebVella.Tefter.Web.Components.LanguageSwitch;
public partial class TfLanguageSwitch : TfBaseComponent
{
	[Inject] protected IState<SessionState> SessionState { get; set; }

	internal bool _visible = false;

	private async Task _select(CultureOption option)
	{
		Dispatcher.Dispatch(new SetUIAction(
		userId: SessionState.Value.UserId,
		spaceId: SessionState.Value.SpaceRouteId,
		spaceDataId: SessionState.Value.SpaceDataRouteId,
		spaceViewId: SessionState.Value.SpaceViewRouteId,
		mode: SessionState.Value.ThemeMode,
		color: SessionState.Value.ThemeColor,
		sidebarExpanded: SessionState.Value.SidebarExpanded,
		cultureOption: option));

		await Task.Delay(5);
		NavigatorExt.ReloadCurrentUrl(Navigator);
	}
}