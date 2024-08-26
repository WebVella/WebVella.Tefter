using WebVella.Tefter.Web.Components.SearchSpaceDialog;
using WebVella.Tefter.Web.Components.SpaceDataFilterManageDialog;
using WebVella.Tefter.Web.Components.SpaceDataManage;
using WebVella.Tefter.Web.Components.SpaceManageDialog;
using WebVella.Tefter.Web.Components.SpaceStateManager;
using WebVella.Tefter.Web.Components.SpaceViewManageDialog;

namespace WebVella.Tefter.UseCases.Space;
public partial class SpaceUseCase
{
	private readonly IIdentityManager _identityManager;
	private readonly ITfSpaceManager _spaceManager;
	private readonly ITfDataProviderManager _dataProviderManager;
	private readonly NavigationManager _navigationManager;
	private readonly IDispatcher _dispatcher;
	private readonly IToastService _toastService;
	private readonly IMessageService _messageService;
	private readonly IStringLocalizer<SpaceUseCase> _loc;

	public SpaceUseCase(
			IIdentityManager identityManager,
			ITfSpaceManager spaceManager,
			ITfDataProviderManager dataProviderManager,
			NavigationManager navigationManager,
			IDispatcher dispatcher,
			IToastService toastService,
			IMessageService messageService,
			IStringLocalizer<SpaceUseCase> loc
			)
	{
		_identityManager = identityManager;
		_spaceManager = spaceManager;
		_dataProviderManager = dataProviderManager;
		_navigationManager = navigationManager;
		_dispatcher = dispatcher;
		_toastService = toastService;
		_messageService = messageService;
		_loc = loc;
	}

	internal async Task Init(Type type)
	{
		if (type == typeof(TfSpaceStateManager)) await InitForState();
		else if (type == typeof(TfSpaceManageDialog)) await InitSpaceManageDialog();
		else if (type == typeof(TfSpaceDataManage)) await InitSpaceDataManage();
		else if (type == typeof(TfSpaceViewManageDialog)) await InitSpaceViewManageDialog();
		else if (type == typeof(TfSearchSpaceDialog)) await InitForSearchSpace();
		else if (type == typeof(TfSpaceDataFilterManageDialog)) await InitSpaceDataFilterManageDialog();
		else throw new Exception($"Type: {type.Name} not supported in SpaceUseCase");
	}

	internal Result<TucSpace> CreateSpaceWithForm(TucSpace space)
	{
		var result = _spaceManager.CreateSpace(space.ToModel());
		if (result.IsFailed) return Result.Fail(new Error("CreateSpaceWithFormAsync failed").CausedBy(result.Errors));
		return Result.Ok(new TucSpace(result.Value));
	}

	internal Result<TucSpace> UpdateSpaceWithForm(TucSpace space)
	{
		var result = _spaceManager.UpdateSpace(space.ToModel());
		if (result.IsFailed) return Result.Fail(new Error("UpdateSpaceWithForm failed").CausedBy(result.Errors));
		return Result.Ok(new TucSpace(result.Value));
	}

	internal TucSpace GetSpace(Guid spaceId)
	{
		var serviceResult = _spaceManager.GetSpace(spaceId);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpace failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		if(serviceResult.Value is null) return null;

		return new TucSpace(serviceResult.Value);
	}

	internal TucSpaceData GetSpaceData(Guid spaceDataId)
	{
		var serviceResult = _spaceManager.GetSpaceData(spaceDataId);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpaceData failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		if(serviceResult.Value is null) return null;

		return new TucSpaceData(serviceResult.Value);
	}

	internal List<TucSpaceData> GetSpaceDataList(Guid spaceId)
	{
		var serviceResult = _spaceManager.GetSpaceDataList(spaceId);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpaceDataList failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		if(serviceResult.Value is null) return new();

		return serviceResult.Value.Select(x=> new TucSpaceData(x)).ToList();
	}
	internal List<TucSpaceView> GetSpaceViewList(Guid spaceId)
	{
		return new List<TucSpaceView>();
		//var serviceResult = _spaceManager.GetSpaceViewList(spaceId);
		//if (serviceResult.IsFailed)
		//{
		//	ResultUtils.ProcessServiceResult(
		//		result: Result.Fail(new Error("GetSpaceDataList failed").CausedBy(serviceResult.Errors)),
		//		toastErrorMessage: "Unexpected Error",
		//		notificationErrorTitle: "Unexpected Error",
		//		toastService: _toastService,
		//		messageService: _messageService
		//	);
		//	return null;
		//}
		//if(serviceResult.Value is null) return new();

		//return serviceResult.Value.Select(x=> new TucSpaceData(x)).ToList();
	}

	internal List<TucDataProvider> GetDataProviderList()
	{
		var serviceResult = _dataProviderManager.GetProviders();
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetProviders failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		if (serviceResult.Value is null) return new();

		return serviceResult.Value.Select(x => new TucDataProvider(x)).ToList();
	}
}
