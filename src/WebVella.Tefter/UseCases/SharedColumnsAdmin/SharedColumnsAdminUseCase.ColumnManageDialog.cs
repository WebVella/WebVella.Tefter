namespace WebVella.Tefter.UseCases.SharedColumnsAdmin;
public partial class SharedColumnsAdminUseCase
{
	internal TucSharedColumnForm SharedColumnForm { get; set; } = new();
	internal List<TucDatabaseColumnTypeInfo> SharedColumnDataTypes { get; set; } = new();
	internal Task InitForColumnManageDialog()
	{
		IsBusy = true;
		return Task.CompletedTask;
	}

	internal Task LoadDataTypeInfoList()
	{
		SharedColumnDataTypes = GetDatabaseColumnTypeInfos().Where(x=> x.CanBeProviderDataType).ToList();
		return Task.CompletedTask;
	}


}
