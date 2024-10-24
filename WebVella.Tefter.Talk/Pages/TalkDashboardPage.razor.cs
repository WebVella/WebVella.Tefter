namespace WebVella.Tefter.Talk.Pages;

public partial class TalkDashboardPage : TucBaseScreenRegionComponent, ITfScreenRegionComponent
{
	public TfScreenRegion ScreenRegion { get { return TfScreenRegion.Pages; } }
	public int Position { get { return 1; } }
	public string Name { get { return "Talk Dashboard"; } }
	public string UrlSlug { get { return "talk-dashboard"; } }

}