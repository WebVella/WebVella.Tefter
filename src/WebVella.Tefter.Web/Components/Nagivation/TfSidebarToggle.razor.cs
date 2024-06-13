using WebVella.Tefter.Web.Store.SessionState;

namespace WebVella.Tefter.Web.Components;
public partial class TfSidebarToggle : TfBaseComponent
{
	[Inject] protected IState<SessionState> SessionState { get; set; }

	private void _toggle()
	{
		Dispatcher.Dispatch(new ToggleSidebarAction());
	}
}