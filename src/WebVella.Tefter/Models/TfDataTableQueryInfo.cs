namespace WebVella.Tefter;

public class TfDataTableQueryInfo 
{
	//later will contain structures for filters
	public TfDataTable DataTable { get; init; }
	public Guid DataProviderId { get; init; }
	public int? Page { get; init; } = null;
	public int? PageSize { get; init; } = null;
	public string Search { get; init; } = null;
	public bool ExcludeSharedColumns { get; set; } = false;

	public TfDataTableQueryInfo(
		TfDataTable dataTable,
		Guid dataProviderId,
		int? page,
		int? pageSize,
		string search,
		bool excludeSharedColumns )
	{
		DataTable = dataTable;
		DataProviderId = dataProviderId;
		Page = page;
		PageSize = pageSize;
		Search = search;
		ExcludeSharedColumns = excludeSharedColumns;
	}
}
