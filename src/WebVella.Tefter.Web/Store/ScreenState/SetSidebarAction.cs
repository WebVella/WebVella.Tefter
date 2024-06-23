namespace WebVella.Tefter.Web.Store.ScreenState;

public record SetSidebarAction
{

	public Guid UserId { get; }
	public bool SidebarExpanded { get; }

	public bool Persist { get; } = true;

	public SetSidebarAction(
		Guid userId,
		bool sidebarExpanded,
		bool persist)
	{
		UserId = userId;
		SidebarExpanded = sidebarExpanded;
		Persist = persist;
	}
}
