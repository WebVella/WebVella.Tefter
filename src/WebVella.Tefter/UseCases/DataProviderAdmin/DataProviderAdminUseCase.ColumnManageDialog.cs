namespace WebVella.Tefter.UseCases.DataProviderAdmin;
public partial class DataProviderAdminUseCase
{

	internal TucDataProviderColumnForm ColumnForm { get; set; } = new();

	internal List<TucDatabaseColumnTypeInfo> ColumnTypes { get; set; } = new List<TucDatabaseColumnTypeInfo>();
	internal List<TucDataProviderColumnSearchTypeInfo> SearchTypes { get; set; } = new();
	internal Task InitForColumnManageDialog()
	{
		ColumnTypes = GetDatabaseColumnTypeInfos();
		foreach (TfDataProviderColumnSearchType item in Enum.GetValues<TfDataProviderColumnSearchType>())
		{
			SearchTypes.Add(new TucDataProviderColumnSearchTypeInfo(item));
		}
		return Task.CompletedTask;
	}


}
