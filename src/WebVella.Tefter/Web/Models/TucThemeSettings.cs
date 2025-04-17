namespace WebVella.Tefter.Web.Models;

public record TucThemeSettings
{
	[JsonIgnore]
	public DesignThemeModes ThemeMode { get; init; } = DesignThemeModes.System;
	
	[JsonPropertyName("mode")]
	public string Mode { get => ThemeMode.ToString(); }

	[JsonIgnore]
	public OfficeColor ThemeColor { get; init; } = OfficeColor.Windows;

	[JsonPropertyName("color")]
	public string Color { get => ThemeColor.ToString(); }
}
