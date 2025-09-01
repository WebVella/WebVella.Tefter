namespace WebVella.Tefter.Assets.Addons;

public partial class AssetsAdminPage : TfBaseComponent, ITfScreenRegionComponent<TfAdminPageScreenRegionContext>
{
	public const string ID = "9cf13acf-8959-499e-aab8-ff2c25a6c97e";
	public const string NAME = "Assets Folders";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "Folder";
	public const int POSITION_RANK = 200;

	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){
			new TfScreenRegionScope(null,new Guid(ID))
		};
	[Parameter]
	public TfAdminPageScreenRegionContext RegionContext { get; set; }
}