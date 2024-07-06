namespace WebVella.Tefter.UseCases.DataProviderAdmin;
public partial class DataProviderAdminUseCase
{

	internal TucDataProviderColumnForm ColumnForm { get; set; } = new();
	internal List<TucDatabaseColumnTypeInfo> ColumnTypes { get; set; } = new List<TucDatabaseColumnTypeInfo>();
	internal Task InitForColumnManageDialog()
	{
		ColumnTypes = GetDatabaseColumnTypeInfos();
		InitColumnTypeDict();
		return Task.CompletedTask;
	}

	internal List<TucDatabaseColumnTypeInfo> GetDatabaseColumnTypeInfos()
	{
		var result = new List<TucDatabaseColumnTypeInfo>();
		var resultColumnType = _dataProviderManager.GetDatabaseColumnTypeInfos();
		if (resultColumnType.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetDatabaseColumnTypeInfos failed").CausedBy(resultColumnType.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return result;
		}

		if (resultColumnType.Value is not null)
		{
			foreach (DatabaseColumnTypeInfo item in resultColumnType.Value)
			{
				result.Add(new TucDatabaseColumnTypeInfo(item));
			}
		}
		return result;

	}
}
