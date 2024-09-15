namespace WebVella.Tefter.Web.Store;

public record InitThemeStateAction
{
	public DesignThemeModes ThemeMode { get; } = DesignThemeModes.System;
	public OfficeColor ThemeColor { get; } = OfficeColor.Excel;

	internal InitThemeStateAction(
		DesignThemeModes themeMode,
		OfficeColor themeColor)
	{
		ThemeMode = themeMode;
		ThemeColor = themeColor;
	}
}
