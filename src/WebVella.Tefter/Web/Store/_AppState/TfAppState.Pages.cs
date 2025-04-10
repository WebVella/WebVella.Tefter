namespace WebVella.Tefter.Web.Store;


public partial record TfAppState
{
	public ReadOnlyCollection<TfScreenRegionComponentMeta> Pages { get; init; }
	
}
