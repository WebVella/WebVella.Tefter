namespace WebVella.Tefter.Web.Store.ThemeState;

[FeatureState]
public record ThemeState
{
	public DesignThemeModes ThemeMode { get; init; } = DesignThemeModes.System;
	public OfficeColor ThemeColor { get; init; } = OfficeColor.Excel;
}
