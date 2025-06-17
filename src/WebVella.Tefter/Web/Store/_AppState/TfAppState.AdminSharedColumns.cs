namespace WebVella.Tefter.Web.Store;

public partial record TfAppState
{
	public List<TucSharedColumn> AdminSharedColumns { get; init; } = new();
	public TucSharedColumn AdminSharedColumn { get; init; } = new();

	public List<TucDatabaseColumnTypeInfo> AdminSharedColumnDataTypes { get; init; } = new();
}
