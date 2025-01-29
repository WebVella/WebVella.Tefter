namespace WebVella.Tefter.Web.Store;


public partial record TfAppState
{
	public ReadOnlyCollection<TfRegionComponentMeta> Pages { get; init; }
	
}
