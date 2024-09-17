namespace WebVella.Tefter.Web.Store;

public partial record TfAppState
{
	public List<TucDataProvider> AdminDataProviders { get; init; } = new();
	public int AdminDataProvidersPage { get; init; }
	public TucDataProvider AdminManagedDataProvider { get; init; }
	internal List<TucDataProviderTypeInfo> DataProviderTypes { get; set; } = new();
}
