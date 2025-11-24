namespace WebVella.Tefter.Models;
public record TfSpaceViewSettings
{
	public int? FreezeStartingNColumns { get; set; } = 0;
	public int? FreezeFinalNColumns { get; set; } = 0;
	public bool CanCreateRows { get; set; } = false;
	public bool CanUpdateRows { get; set; } = false;
	public bool CanDeleteRows { get; set; } = false;
	public string? MainTabLabel { get; set; } = null;
	public string? MainTabFluentIcon { get; set; } = null;
	public TfColor? MainTabColor { get; set; } = null;
	public List<TfColoringRule> ColoringRules { get; set; } = new();
}
