namespace WebVella.Tefter.Web.Components;
public partial class TfSidebarToggle : TfBaseComponent
{
	[Inject] protected IState<UserState> UserState { get; set; }
	[Inject] protected IState<SessionState> SessionState { get; set; }
	private void _toggle()
	{
		Dispatcher.Dispatch(new SetUIAction(UserState.Value.User.Id,SessionState.Value.ThemeMode,SessionState.Value.ThemeColor, !SessionState.Value.SidebarExpanded));
	}
}