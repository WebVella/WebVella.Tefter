namespace WebVella.Tefter.Web.Store;

public record SetThemeAction
{
	public DesignThemeModes ThemeMode { get; } = DesignThemeModes.System;
	public OfficeColor ThemeColor { get; } = OfficeColor.Excel;
	public TucUser CurrentUser { get; }

	public SetThemeAction(TucUser currentUser,
		DesignThemeModes themeMode, 
		OfficeColor themeColor)
	{
		CurrentUser = currentUser;
		ThemeMode = themeMode;
		ThemeColor = themeColor;
	}
}
