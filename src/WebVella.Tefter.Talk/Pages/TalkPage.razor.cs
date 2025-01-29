namespace WebVella.Tefter.Talk.Pages;


public partial class TalkPage : TfBaseComponent, ITfRegionComponent<TfPageComponentContext>
{
	public Guid Id { get; init; } = new Guid("beb9a070-49b0-4c49-b57e-7f110da4dccd");
	public int PositionRank { get; init; } = 90;
	public string Name { get; init; } = "Talk Dashboard";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "Folder";
	public List<TfRegionComponentScope> Scopes { get; init; } = new List<TfRegionComponentScope>(){
		new TfRegionComponentScope(null,new Guid("beb9a070-49b0-4c49-b57e-7f110da4dccd"))
	};
	[Parameter]
	public TfPageComponentContext Context { get; init; }

}