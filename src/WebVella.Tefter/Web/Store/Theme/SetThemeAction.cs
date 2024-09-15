namespace WebVella.Tefter.Web.Store;

public record SetThemeAction
{
	public Guid UserId { get; }
	public DesignThemeModes ThemeMode { get; } = DesignThemeModes.System;
	public OfficeColor ThemeColor { get; } = OfficeColor.Excel;

	public SetThemeAction(Guid userId,
		DesignThemeModes themeMode, 
		OfficeColor themeColor)
	{
		UserId = userId;
		ThemeMode = themeMode;
		ThemeColor = themeColor;
	}
}
