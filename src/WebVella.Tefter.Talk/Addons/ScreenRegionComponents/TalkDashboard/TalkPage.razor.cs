namespace WebVella.Tefter.Talk.Addons;


public partial class TalkPage : TfBaseComponent, ITfRegionComponent<TfPageScreenRegionContext>
{

	public const string ID = "beb9a070-49b0-4c49-b57e-7f110da4dccd";
	public const string NAME = "Talk Dashboard";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "Folder";
	public const int POSITION_RANK = 90;
	public Guid Id { get; init; } = new Guid(ID);
	public string Name { get; init; } = NAME;
	public string Description { get; init; } = DESCRIPTION;
	public string FluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){
		new TfScreenRegionScope(null,new Guid(ID))
	};
	[Parameter]
	public TfPageScreenRegionContext RegionContext { get; init; }

}