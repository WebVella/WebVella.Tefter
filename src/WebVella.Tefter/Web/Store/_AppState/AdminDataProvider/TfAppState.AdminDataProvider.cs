namespace WebVella.Tefter.Web.Store;

public partial record TfAppState
{
	public List<TucDataProvider> AdminDataProviders { get; init; } = new();
	public TucDataProvider AdminDataProvider { get; init; }
	internal List<TucDataProviderTypeInfo> DataProviderTypes { get; set; } = new();
	internal List<TucDataProviderSyncTask> DataProviderSyncTasks { get; set; } = new();

	internal TfDataTable AdminDataProviderData { get; set; }
	internal int AdminDataProviderDataPage { get; set; }
}
