namespace WebVella.Tefter.Talk.Addons;

public partial class TalkHomePage : TfBaseComponent, ITfScreenRegionAddon<TfPageScreenRegion>
{

	public const string ID = "615222b9-de51-4109-84e7-6a26cc538cfb";
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
	public TfPageScreenRegion RegionContext { get; set; }

}