namespace WebVella.Tefter.Web.Store.ScreenState;

public record SetSidebarAction
{
	public Guid UserId { get; }
	public bool SidebarExpanded { get; }

	internal SetSidebarAction(
		Guid userId,
		bool sidebarExpanded)
	{
		UserId = userId;
		SidebarExpanded = sidebarExpanded;
	}
}
