namespace WebVella.Tefter.Web.Store.SessionState;

public class SetUIAction
{
    public Guid UserId { get; set; }
    public DesignThemeModes ThemeMode { get; } = DesignThemeModes.System;
    public OfficeColor ThemeColor { get; } = OfficeColor.Excel;
    public bool SidebarExpanded { get; } = true;

    public SetUIAction(Guid userId, DesignThemeModes mode, OfficeColor color, bool sidebarExpanded)
    {
        UserId = userId;
        ThemeMode = mode;
        ThemeColor = color;
        SidebarExpanded = sidebarExpanded;
    }
}
