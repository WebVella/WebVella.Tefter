namespace WebVella.Tefter.Assets.Pages;

public partial class AssetsDashboardPage : TucBaseScreenRegionComponent //BOZ: not implemented yet, ITfScreenRegionComponent
{
	public Guid Id { get { return new Guid("51711fb0-524e-470b-8567-11d61fe60ca1"); } }
	public TfScreenRegion ScreenRegion { get { return TfScreenRegion.Pages; } }
	public int Position { get { return 2; } }
	public string Name { get { return "Assets Dashboard"; } }
	public string UrlSlug { get { return "assets-dashboard"; } }

}