namespace WebVella.Tefter.Web.Models;

public record TucThemeSettings
{
	[JsonIgnore]
	public DesignThemeModes ThemeMode { get; init; } = DesignThemeModes.System;
	
	[JsonPropertyName("mode")]
	public string Mode { get => ThemeMode.ToString(); }

	[JsonIgnore]
	public TfColor ThemeColor { get; init; } = TfColor.Emerald500;

	[JsonPropertyName("color")]
	public string Color { get => ThemeColor.ToString(); }
}
