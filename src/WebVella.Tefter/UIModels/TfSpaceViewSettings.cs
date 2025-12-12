namespace WebVella.Tefter.Models;

public record TfSpaceViewSettings
{
	public int? FreezeStartingNColumns { get; set; } = 0;
	public int? FreezeFinalNColumns { get; set; } = 0;
	public bool CanCreateRows { get; set; } = false;
	public bool CanUpdateRows { get; set; } = false;
	public bool CanDeleteRows { get; set; } = false;
	public List<TfColoringRule> ColoringRules { get; set; } = new();
	public TfSpaceViewFilterType FitlerType { get; set; } = TfSpaceViewFilterType.ToolbarFilter;
}

public enum TfSpaceViewFilterType
{
	[Description("present in the top toolbar")]
	ToolbarFilter = 0,
	[Description("present in the grid header")]
	GridFilter = 1
}
