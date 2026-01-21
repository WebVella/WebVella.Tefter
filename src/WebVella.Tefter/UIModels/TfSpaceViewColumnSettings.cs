namespace WebVella.Tefter.Models;

public record TfSpaceViewColumnSettings
{
	[JsonPropertyName("width")]
	public short? Width { get; set; }
	[JsonPropertyName("filterPresentation")]
	public TfSpaceViewColumnSettingsFilterPresentation FilterPresentation { get; set; } = TfSpaceViewColumnSettingsFilterPresentation.VisibleWithOptions;
	[JsonPropertyName("defaultFilter")]
	public string? DefaultComparisonMethodDescription { get; set; } = null;
}

public enum TfSpaceViewColumnSettingsFilterPresentation
{
	[Description("visible with comparison options")]
	VisibleWithOptions = 0,
	[Description("visible with only default")]
	VisibleWithDefault = 1,	
	[Description("hidden")]
	Hidden = 2,	
}
