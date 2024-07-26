namespace WebVella.Tefter.UseCases.DataProviderAdmin;
public partial class DataProviderAdminUseCase
{

	internal TucDataProviderAuxColumnForm AuxColumnForm { get; set; } = new();

	internal List<TucDatabaseColumnTypeInfo> AuxColumnTypes { get; set; } = new List<TucDatabaseColumnTypeInfo>();
	internal List<TucDataProviderColumnSearchTypeInfo> AuxSearchTypes { get; set; } = new();
	internal Task InitForAuxColumnManageDialog()
	{
		ColumnTypes = GetDatabaseColumnTypeInfos();
		foreach (TfDataProviderColumnSearchType item in Enum.GetValues<TfDataProviderColumnSearchType>())
		{
			SearchTypes.Add(new TucDataProviderColumnSearchTypeInfo(item));
		}
		return Task.CompletedTask;
	}


}
