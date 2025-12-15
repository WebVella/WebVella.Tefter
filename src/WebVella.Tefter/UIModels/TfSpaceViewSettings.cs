namespace WebVella.Tefter.Models;

public record TfSpaceViewSettings
{
	public int? FreezeStartingNColumns { get; set; } = 0;
	public int? FreezeFinalNColumns { get; set; } = 0;
	public List<TfColoringRule> ColoringRules { get; set; } = new();
	public TfSpaceViewFilterType FitlerType { get; set; } = TfSpaceViewFilterType.ToolbarFilter;

	public List<Guid> CanCreateRoles { get; set; } = new();
	public List<Guid> CanUpdateRoles { get; set; } = new();
	public List<Guid> CanDeleteRoles { get; set; } = new();

}

public enum TfSpaceViewFilterType
{
	[Description("present in the top toolbar")]
	ToolbarFilter = 0,
	[Description("present in the grid header")]
	GridFilter = 1
}
