using WebVella.Tefter.Api;
using WebVella.Tefter.Web.Components.AdminSharedColumns;
using WebVella.Tefter.Web.Components.SharedColumnManageDialog;


namespace WebVella.Tefter.UseCases.SharedColumnsAdmin;
public partial class SharedColumnsAdminUseCase
{
	private readonly ITfDataProviderManager _dataProviderManager;
	private readonly ITfSharedColumnsManager _sharedColumnsManager;
	private readonly IDataManager _dataManager;
	private readonly NavigationManager _navigationManager;
	private readonly IToastService _toastService;
	private readonly IMessageService _messageService;
	private readonly IDispatcher _dispatcher;
	public SharedColumnsAdminUseCase(
		ITfDataProviderManager dataProviderManager,
		ITfSharedColumnsManager sharedColumnsManager,
		IDataManager dataManager,
		NavigationManager navigationManager,
		IToastService toastService,
		IMessageService messageService,
		IDispatcher dispatcher
	)
	{
		_dataProviderManager = dataProviderManager;
		_sharedColumnsManager = sharedColumnsManager;
		_dataManager = dataManager;
		_navigationManager = navigationManager;
		_toastService = toastService;
		_messageService = messageService;
		_dispatcher = dispatcher;
	}

	internal bool IsBusy { get; set; } = false;
	internal bool IsListBusy { get; set; } = false;

	internal async Task Init(Type type)
	{
		if (type == typeof(TfAdminSharedColumns)) await InitForColumnList();
		else if (type == typeof(TfSharedColumnManageDialog)) await InitForColumnManageDialog();
		else throw new Exception($"Type: {type.Name} not supported in AuxColumnsAdminUseCase");

	}

	//column
	internal List<TucSharedColumn> GetSharedColumns()
	{
		var result = new List<TucSharedColumn>();
		var tfResult = _sharedColumnsManager.GetSharedColumns();
		if (tfResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSharedColumns failed").CausedBy(tfResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return result;
		}

		if (tfResult.Value is not null)
		{
			foreach (TfSharedColumn item in tfResult.Value)
			{
				result.Add(new TucSharedColumn(item));
			}
		}
		return result;

	}

	internal TucSharedColumn GetSharedColumn(Guid id)
	{
		var result = new TucSharedColumn();
		var tfResult = _sharedColumnsManager.GetSharedColumn(id);
		if (tfResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSharedColumn failed").CausedBy(tfResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return result;
		}

		if (tfResult.Value is not null)
		{
			return new TucSharedColumn(tfResult.Value);
		}
		return null;

	}

	internal Result<TucSharedColumn> CreateSharedColumn(TucSharedColumnForm form)
	{
		if (form.Id == Guid.Empty)
			form = form with { Id = Guid.NewGuid() };
		var result = _sharedColumnsManager.CreateSharedColumn(form.ToModel());
		if (result.IsFailed)
			return Result.Fail(new Error("CreateDataProviderColumn failed").CausedBy(result.Errors));

		return Result.Ok(GetSharedColumn(form.Id));
	}

	internal Result<TucSharedColumn> UpdateSharedColumn(TucSharedColumnForm form)
	{
		var result = _sharedColumnsManager.UpdateSharedColumn(form.ToModel());
		if (result.IsFailed)
			return Result.Fail(new Error("UpdateDataProviderColumn failed").CausedBy(result.Errors));

		return Result.Ok(GetSharedColumn(form.Id));
	}

	internal Result DeleteSharedColumn(Guid columnId)
	{
		var result = _sharedColumnsManager.DeleteSharedColumn(columnId);
		if (result.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetDatabaseColumnTypeInfos failed").CausedBy(result.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return result;
		}

		return Result.Ok();
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
