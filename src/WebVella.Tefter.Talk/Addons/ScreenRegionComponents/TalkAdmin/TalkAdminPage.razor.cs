namespace WebVella.Tefter.Talk.Addons;

public partial class TalkAdminPage : TfBaseComponent, ITfScreenRegionComponent<TfAdminPageScreenRegionContext>
{

	public const string ID = "15f22367-7c8d-4971-9950-d7b1ff51f678";
	public const string NAME = "Talk Channels";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "CommentMultiple";
	public const int POSITION_RANK = 300;
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