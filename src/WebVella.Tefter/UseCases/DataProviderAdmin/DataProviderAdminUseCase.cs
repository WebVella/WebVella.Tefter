namespace WebVella.Tefter.UseCases.DataProviderAdmin;
public partial class DataProviderAdminUseCase
{
	private readonly ITfDataProviderManager _dataProviderManager;
	private readonly IDataManager _dataManager;
	private readonly NavigationManager _navigationManager;
	private readonly IToastService _toastService;
	private readonly IMessageService _messageService;
	private readonly IDispatcher _dispatcher;
	public DataProviderAdminUseCase(
		ITfDataProviderManager dataProviderManager,
		IDataManager dataManager,
		NavigationManager navigationManager,
		IToastService toastService,
		IMessageService messageService,
		IDispatcher dispatcher
	)
	{
		_dataProviderManager = dataProviderManager;
		_dataManager = dataManager;
		_navigationManager = navigationManager;
		_toastService = toastService;
		_messageService = messageService;
		_dispatcher = dispatcher;
	}

	internal bool IsBusy { get; set; } = false;

	internal async Task Init(Type type)
	{
		if (type == typeof(TfAdminDataProviderAux)) await InitForAuxColumns();
		else if (type == typeof(TfAdminDataProviderDetails)) await InitForDetails();
		else if (type == typeof(TfAdminDataProviderData)) await InitForData();
		else if (type == typeof(TfAdminDataProviderSynchronization)) await InitForSynchronization();
		else if (type == typeof(TfDataProviderManageDialog)) await InitForProviderManageDialog();
		else if (type == typeof(TfAdminDataProviderNavigation)) await InitForNavigation();
		else if (type == typeof(TfAdminDataProviderStateManager)) await InitForState();
		else if (type == typeof(TfAdminDataProviderDetailsActions)) await InitForDetailsActions();
		else if (type == typeof(TfAdminDataProviderKeys)) await InitForKeys();
		else if (type == typeof(TfAdminDataProviderSchema)) await InitForSchema();
		else if (type == typeof(TfDataProviderColumnManageDialog)) await InitForColumnManageDialog();
		else if (type == typeof(TfDataProviderKeyManageDialog)) await InitForKeyManageDialog();
		else if (type == typeof(TfDataProviderAuxColumnManageDialog)) await InitForAuxColumnManageDialog();
		else if (type == typeof(TfDataProviderSyncLogDialog)) await InitSyncLogDialog();
		else throw new Exception($"Type: {type.Name} not supported in DataProviderAdminUseCase");

	}

	//provider
	internal Result<TucDataProvider> GetProvider(Guid providerId)
	{
		var result = _dataProviderManager.GetProvider(providerId);
		if (result.IsFailed)
			return Result.Fail(new Error("GetProvider failed").CausedBy(result.Errors));
		if (result.Value is null)
			return Result.Fail(new Error("GetProvider - no provider found"));

		return Result.Ok(new TucDataProvider(result.Value));
	}
	internal Result DeleteDataProvider(Guid providerId)
	{
		var result = _dataProviderManager.DeleteDataProvider(providerId);
		if (result.IsFailed)
			return Result.Fail(new Error("DeleteDataProvider failed").CausedBy(result.Errors));

		return Result.Ok();
	}
	internal Result<List<TucDatabaseColumnTypeInfo>> GetDbTypesForProviderSourceDataTable(Guid providerId, string dataType)
	{
		var allProviders = _dataProviderManager.GetProviderTypes();
		if (allProviders.IsFailed) return Result.Fail(new Error("GetProviderTypes failed"));
		if (allProviders.Value is null) return Result.Fail(new Error("provider not found"));
		var provider = allProviders.Value.FirstOrDefault(x => x.Id == providerId);
		if (provider is null) return Result.Fail(new Error("provider not found"));
		var providerDbType = provider.GetDatabaseColumnTypesForSourceDataType(dataType);
		return Result.Ok(providerDbType.Select(x => new TucDatabaseColumnTypeInfo(x)).ToList());

	}

	//column
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

	internal Result<TucDataProvider> CreateDataProviderColumn(TucDataProviderColumnForm form)
	{
		var result = _dataProviderManager.CreateDataProviderColumn(form.ToModel());
		if (result.IsFailed)
			return Result.Fail(new Error("CreateDataProviderColumn failed").CausedBy(result.Errors));

		return Result.Ok(new TucDataProvider(result.Value));
	}

	internal Result<TucDataProvider> UpdateDataProviderColumn(TucDataProviderColumnForm form)
	{
		var result = _dataProviderManager.UpdateDataProviderColumn(form.ToModel());
		if (result.IsFailed)
			return Result.Fail(new Error("UpdateDataProviderColumn failed").CausedBy(result.Errors));

		return Result.Ok(new TucDataProvider(result.Value));
	}
	internal Result<TucDataProvider> DeleteDataProviderColumn(Guid columnId)
	{
		var result = _dataProviderManager.DeleteDataProviderColumn(columnId);
		if (result.IsFailed)
			return Result.Fail(new Error("DeleteDataProviderColumn failed").CausedBy(result.Errors));

		return Result.Ok(new TucDataProvider(result.Value));
	}

	//shared column

	internal Result<bool> GetDataProviderSharedColumns()
	{
		return Result.Ok(true);
	}

	//Key

	internal Result<TucDataProvider> CreateDataProviderKey(TucDataProviderSharedKeyForm form)
	{
		var result = _dataProviderManager.CreateDataProviderSharedKey(form.ToModel());
		if (result.IsFailed)
			return Result.Fail(new Error("CreateDataProviderSharedKey failed").CausedBy(result.Errors));

		return Result.Ok(new TucDataProvider(result.Value));
	}

	internal Result<TucDataProvider> UpdateDataProviderKey(TucDataProviderSharedKeyForm form)
	{
		var result = _dataProviderManager.UpdateDataProviderSharedKey(form.ToModel());
		if (result.IsFailed)
			return Result.Fail(new Error("UpdateDataProviderColumn failed").CausedBy(result.Errors));

		return Result.Ok(new TucDataProvider(result.Value));
	}
	internal Result<TucDataProvider> DeleteDataProviderSharedKey(Guid keyId)
	{
		var result = _dataProviderManager.DeleteDataProviderSharedKey(keyId);
		if (result.IsFailed)
			return Result.Fail(new Error("DeleteDataProviderSharedKey failed").CausedBy(result.Errors));

		return Result.Ok(new TucDataProvider(result.Value));
	}

	//Data
	internal Result<TfDataTable> GetProviderRows(
		Guid providerId,
		string search = null,
		int? page = null,
		int? pageSize = null
	)
	{
		var result = _dataProviderManager.GetProvider(providerId);
		if (result.IsFailed)
			return Result.Fail(new Error("GetProvider failed").CausedBy(result.Errors));

		if (result.Value is null)
			return Result.Fail(new Error("Provider not found"));

		var dtResult = _dataManager.QueryDataProvider(
			provider: result.Value,
			search: search,
			page: page,
			pageSize: pageSize);

		if (dtResult.IsFailed)
			return Result.Fail(new Error("GetProviderRows failed").CausedBy(dtResult.Errors));

		return dtResult;
	}

}
