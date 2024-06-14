using WebVella.Tefter.Web.Store.UserState;

namespace WebVella.Tefter.Web.Components;
public partial class TfSidebarToggle : TfBaseComponent
{
	[Inject] protected IState<UserState> UserState { get; set; }

	private void _toggle()
	{
		Dispatcher.Dispatch(new ToggleSidebarAction());
	}
}