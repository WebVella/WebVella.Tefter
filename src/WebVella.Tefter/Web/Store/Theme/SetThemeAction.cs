namespace WebVella.Tefter.Web.Store;

public record SetThemeAction : TfBaseAction
{
	public DesignThemeModes ThemeMode { get; } = DesignThemeModes.System;
	public OfficeColor ThemeColor { get; } = OfficeColor.Excel;
	public TucUser CurrentUser { get; }

	public SetThemeAction(
		TfBaseComponent component,
		TucUser currentUser,
		DesignThemeModes themeMode, 
		OfficeColor themeColor)
	{
		Component = component;
		CurrentUser = currentUser;
		ThemeMode = themeMode;
		ThemeColor = themeColor;
	}
}
