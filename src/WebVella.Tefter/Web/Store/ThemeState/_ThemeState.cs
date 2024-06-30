namespace WebVella.Tefter.Web.Store.ThemeState;

[FeatureState]
public record ThemeState
{
	[JsonIgnore]
	internal StateUseCase UseCase { get; init; }
	public DesignThemeModes ThemeMode { get; init; } = DesignThemeModes.System;
	public OfficeColor ThemeColor { get; init; } = OfficeColor.Excel;
}
