namespace WebVella.Tefter.Talk.Pages;

public partial class TalkDashboardPage : TucBaseScreenRegionComponent //BOZ: not implemented yet, ITfScreenRegionComponent
{
	public Guid Id { get { return new Guid("beb9a070-49b0-4c49-b57e-7f110da4dccd"); } }
	public TfScreenRegion ScreenRegion { get { return TfScreenRegion.Pages; } }
	public int Position { get { return 1; } }
	public string Name { get { return "Talk Dashboard"; } }
	public string UrlSlug { get { return "talk-dashboard"; } }

}