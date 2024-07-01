namespace WebVella.Tefter.UseCases.Models;

public record TucThemeSettings
{
	[JsonIgnore]
	public DesignThemeModes ThemeMode { get; init; } = DesignThemeModes.System;
	
	[JsonPropertyName("mode")]
	public string Mode { get => ThemeMode.ToString(); }

	[JsonIgnore]
	public OfficeColor ThemeColor { get; init; } = OfficeColor.Excel;

	[JsonPropertyName("color")]
	public string Color { get => ThemeColor.ToString(); }
}
