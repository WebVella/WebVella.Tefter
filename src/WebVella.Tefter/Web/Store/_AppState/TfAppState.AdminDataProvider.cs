namespace WebVella.Tefter.Web.Store;

public partial record TfAppState
{
	public List<TucDataProvider> AdminDataProviders { get; init; } = new();
	public TucDataProvider AdminDataProvider { get; init; }
	internal List<TucDataProviderTypeInfo> DataProviderTypes { get; set; } = new();
	internal List<TucDataProviderSyncTask> DataProviderSyncTasks { get; set; } = new();
	public string AdminDataProviderDataSearch { get; init; } = null;
	public int AdminDataProviderDataPage { get; init; } = 1;
	public int AdminDataProviderDataPageSize { get; init; } = TfConstants.PageSize;
	internal TfDataTable AdminDataProviderData { get; set; }

}
