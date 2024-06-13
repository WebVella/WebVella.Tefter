namespace WebVella.Tefter.Demo.Store;

public partial class ChangeThemeAction
{
	public DesignThemeModes ThemeMode { get; set; } = DesignThemeModes.System;
	public OfficeColor ThemeColor { get; set; } = OfficeColor.Excel;
}
