namespace WebVella.Tefter.Web.Store;

public record SetSidebarAction
{
	public bool SidebarExpanded { get; }

	internal SetSidebarAction(
		bool sidebarExpanded)
	{
		SidebarExpanded = sidebarExpanded;
	}
}
