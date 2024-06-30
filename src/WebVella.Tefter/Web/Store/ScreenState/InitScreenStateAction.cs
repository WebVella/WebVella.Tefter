namespace WebVella.Tefter.Web.Store.ScreenState;

public record InitScreenStateAction
{
	public bool SidebarExpanded { get; }

	internal InitScreenStateAction(
		bool sidebarExpanded)
	{
		SidebarExpanded = sidebarExpanded;
	}
}
