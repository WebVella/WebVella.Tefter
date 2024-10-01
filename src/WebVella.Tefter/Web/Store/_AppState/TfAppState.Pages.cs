namespace WebVella.Tefter.Web.Store;


public partial record TfAppState
{
	public List<TucScreenRegionComponentMeta> Pages { get; init; } = new();
	
}
