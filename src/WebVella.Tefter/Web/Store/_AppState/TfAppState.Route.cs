namespace WebVella.Tefter.Web.Store;


public partial record TfAppState
{
	public TucRouteState Route { get; init; } = null;
}
