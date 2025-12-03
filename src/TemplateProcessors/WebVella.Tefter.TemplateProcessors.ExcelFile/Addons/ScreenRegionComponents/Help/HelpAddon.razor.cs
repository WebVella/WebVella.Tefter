using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.ExcelFile.Addons;

public partial class HelpAddon : TfBaseComponent, 
	ITfScreenRegionAddon<TfTemplateProcessorHelpScreenRegion>
{
	public const string ID = "c1090557-af1e-48c9-844d-51c90170df24";
	public const string NAME = "Excel Template Help";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "PuzzlePiece";
	public const int POSITION_RANK = 1000;
	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(ExcelFileTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorHelpScreenRegion RegionContext { get; set; }

}