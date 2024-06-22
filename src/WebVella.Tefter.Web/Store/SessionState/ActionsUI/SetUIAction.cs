namespace WebVella.Tefter.Web.Store.SessionState;

public class SetUIAction
{
    public Guid UserId { get; set; }
	public Guid? SpaceId { get; }
	public Guid? SpaceDataId { get; }
	public Guid? SpaceViewId { get; }
	public DesignThemeModes ThemeMode { get; } = DesignThemeModes.System;
    public OfficeColor ThemeColor { get; } = OfficeColor.Excel;
    public bool SidebarExpanded { get; } = true;
    public CultureOption CultureOption { get; }

    public SetUIAction(Guid userId, 
		Guid? spaceId, Guid? spaceDataId, Guid? spaceViewId,
		DesignThemeModes mode, OfficeColor color, 
        bool sidebarExpanded, CultureOption cultureOption)
    {
        UserId = userId;
        SpaceId = spaceId;
        SpaceDataId = spaceDataId;
        SpaceViewId = spaceViewId;
        ThemeMode = mode;
        ThemeColor = color;
        SidebarExpanded = sidebarExpanded;
        CultureOption = cultureOption;
    }
}
