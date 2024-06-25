namespace WebVella.Tefter.Web.Store.ThemeState;

public record SetThemeAction
{
	public Guid UserId { get; }
	public DesignThemeModes ThemeMode { get; } = DesignThemeModes.System;
	public OfficeColor ThemeColor { get; } = OfficeColor.Excel;
	public bool Persist { get; } = true;

	public SetThemeAction(Guid userId,
		DesignThemeModes themeMode, 
		OfficeColor themeColor,
		bool persist)
	{
		UserId = userId;
		ThemeMode = themeMode;
		ThemeColor = themeColor;
		Persist = persist;
	}
}
