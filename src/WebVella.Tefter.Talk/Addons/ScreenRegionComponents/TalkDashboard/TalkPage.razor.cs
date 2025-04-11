namespace WebVella.Tefter.Talk.Addons;


public partial class TalkPage : TfBaseComponent, ITfRegionComponent<TfPageScreenRegion>
{
	public Guid Id { get; init; }
	public int PositionRank { get; init; }
	public string Name { get; init; }
	public string Description { get; init; }
	public string FluentIconName { get; init; }
	public List<TfScreenRegionScope> Scopes { get; init; }
	[Parameter]
	public TfPageScreenRegion Context { get; init; }

	public TalkPage() : base()
	{
		var componentId = new Guid("beb9a070-49b0-4c49-b57e-7f110da4dccd");
		Id = componentId;
		PositionRank = 90;
		Name = "Talk Dashboard";
		Description = "";
		FluentIconName = "Folder";
		Scopes = new List<TfScreenRegionScope>(){
		new TfScreenRegionScope(null,componentId)
	};
	}
}