namespace WebVella.Tefter.Web.Store.UserState;

public class SetThemeAction
{
    public DesignThemeModes ThemeMode { get; } = DesignThemeModes.System;
    public OfficeColor ThemeColor { get; } = OfficeColor.Excel;

    public SetThemeAction(DesignThemeModes mode, OfficeColor color)
    {
        ThemeMode = mode;
        ThemeColor = color;
    }
}
