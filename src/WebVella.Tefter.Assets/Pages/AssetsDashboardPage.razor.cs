namespace WebVella.Tefter.Assets.Pages;

public partial class AssetsDashboardPage : TucBaseScreenRegionComponent //BOZ: not implemented yet, ITfScreenRegionComponent
{
	public TfScreenRegion ScreenRegion { get { return TfScreenRegion.Pages; } }
	public int Position { get { return 2; } }
	public string Name { get { return "Assets Dashboard"; } }
	public string UrlSlug { get { return "assets-dashboard"; } }

}