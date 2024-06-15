namespace WebVella.Tefter.Web.Components;
public partial class TfSidebarToggle : TfBaseComponent
{
	private void _toggle()
	{
		Dispatcher.Dispatch(new ToggleSidebarAction());
	}
}