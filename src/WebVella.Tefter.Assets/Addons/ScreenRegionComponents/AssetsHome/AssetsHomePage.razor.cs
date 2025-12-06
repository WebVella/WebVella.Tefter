namespace WebVella.Tefter.Assets.Addons;

public partial class AssetsHomePage : TfBaseComponent, ITfScreenRegionAddon<TfPageScreenRegion>
{
	public const string ID = "532197c1-9d6d-42c8-baca-83275678f007";
	public const string NAME = "Assets Folders";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "Folder";
	public const int POSITION_RANK = 500;

	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){
			new TfScreenRegionScope(null,new Guid(ID))
		};
	[Parameter]
	public TfPageScreenRegion RegionContext { get; set; }
}