using WebVella.Tefter.Web.Components.AdminDataProviderAux;
using WebVella.Tefter.Web.Components.AdminDataProviderData;
using WebVella.Tefter.Web.Components.AdminDataProviderDetails;
using WebVella.Tefter.Web.Components.AdminDataProviderDetailsActions;
using WebVella.Tefter.Web.Components.AdminDataProviderKeys;
using WebVella.Tefter.Web.Components.AdminDataProviderNavigation;
using WebVella.Tefter.Web.Components.AdminDataProviderSchema;
using WebVella.Tefter.Web.Components.AdminDataProviderStateManager;
using WebVella.Tefter.Web.Components.DataProviderColumnManageDialog;
using WebVella.Tefter.Web.Components.DataProviderManageDialog;

namespace WebVella.Tefter.UseCases.DataProviderAdmin;
public partial class DataProviderAdminUseCase
{
	private readonly ITfDataProviderManager _dataProviderManager;
	private readonly NavigationManager _navigationManager;
	private readonly IToastService _toastService;
	private readonly IMessageService _messageService;
	private readonly IDispatcher _dispatcher;
	public DataProviderAdminUseCase(
		ITfDataProviderManager dataProviderManager,
		NavigationManager navigationManager,
		IToastService toastService,
		IMessageService messageService,
		IDispatcher dispatcher
	)
	{
		_dataProviderManager = dataProviderManager;
		_navigationManager = navigationManager;
		_toastService = toastService;
		_messageService = messageService;
		_dispatcher = dispatcher;
	}

	internal async Task Init(Type type)
	{
		if (type == typeof(TfAdminDataProviderAux)) await InitForAuxColumns();
		else if (type == typeof(TfAdminDataProviderDetails)) await InitForDetails();
		else if (type == typeof(TfAdminDataProviderData)) await InitForData();
		else if (type == typeof(TfDataProviderManageDialog)) await InitForProviderManageDialog();
		else if (type == typeof(TfAdminDataProviderNavigation)) await InitForNavigation();
		else if (type == typeof(TfAdminDataProviderStateManager)) await InitForState();
		else if (type == typeof(TfAdminDataProviderDetailsActions)) await InitForDetailsActions();
		else if (type == typeof(TfAdminDataProviderKeys)) await InitForKeys();
		else if (type == typeof(TfAdminDataProviderSchema)) await InitForSchema();
		else if (type == typeof(TfDataProviderColumnManageDialog)) await InitForColumnManageDialog();

		else throw new Exception($"Type: {type.Name} not supported in DataProviderAdminUseCase");

	}

	public Result<TucDataProvider> GetProvider(Guid providerId)
	{
		var result = _dataProviderManager.GetProvider(providerId);
		if (result.IsFailed)
			return Result.Fail(new Error("GetProvider failed").CausedBy(result.Errors));
		if (result.Value is null)
			return Result.Fail(new Error("GetProvider - no provider found"));

		return Result.Ok(new TucDataProvider(result.Value));
	}
	public Result DeleteDataProvider(Guid providerId)
	{
		var result = _dataProviderManager.DeleteDataProvider(providerId);
		if (result.IsFailed)
			return Result.Fail(new Error("DeleteDataProvider failed").CausedBy(result.Errors));

		return Result.Ok();
	}
	public Result<List<TucDatabaseColumnTypeInfo>> GetDbTypesForProviderSourceDataTable(Guid providerId, string dataType)
	{
		var allProviders = _dataProviderManager.GetProviderTypes();
		if (allProviders.IsFailed) return Result.Fail(new Error("GetProviderTypes failed"));
		if (allProviders.Value is null) return Result.Fail(new Error("provider not found"));
		var provider = allProviders.Value.FirstOrDefault(x => x.Id == providerId);
		if (provider is null) return Result.Fail(new Error("provider not found"));
		var providerDbType = provider.GetDatabaseColumnTypesForSourceDataType(dataType);
		return Result.Ok(providerDbType.Select(x => new TucDatabaseColumnTypeInfo(x)).ToList());

	}

}
