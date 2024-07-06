namespace WebVella.Tefter.UseCases.DataProviderAdmin;
public partial class DataProviderAdminUseCase
{
	internal Dictionary<int, TucDatabaseColumnTypeInfo> ColumnTypeDict { get; set; } = new();


	internal Task InitForSchema()
	{
		InitColumnTypeDict();
		return Task.CompletedTask;
	}

	internal void InitColumnTypeDict()
	{
		var dbTypesResult = _dataProviderManager.GetDatabaseColumnTypeInfos();
		if (dbTypesResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetDatabaseColumnTypeInfos failed").CausedBy(dbTypesResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return;
		}
		if (dbTypesResult.Value is not null)
		{
			foreach (DatabaseColumnTypeInfo item in dbTypesResult.Value)
			{
				if (item.Type == DatabaseColumnType.AutoIncrement) continue;
				ColumnTypeDict[(int)item.Type] = new TucDatabaseColumnTypeInfo(item.Type);
			}
		}
	}
}
