using Microsoft.AspNetCore.Localization;
using WebVella.Tefter.Api;

namespace WebVella.Tefter.UseCases.AppStart;

internal partial class AppStateUseCase
{
	private readonly AuthenticationStateProvider _authenticationStateProvider;
	private readonly IJSRuntime _jsRuntime;
	private readonly IIdentityManager _identityManager;
	private readonly ITfDataProviderManager _dataProviderManager;
	private readonly ITfSharedColumnsManager _sharedColumnsManager;
	private readonly IDataManager _dataManager;
	private readonly ITfSpaceManager _spaceManager;
	private readonly NavigationManager _navigationManager;
	private readonly IToastService _toastService;
	private readonly IMessageService _messageService;
	private readonly IStringLocalizer<AppStateUseCase> LOC;


	public AppStateUseCase(
		AuthenticationStateProvider authenticationStateProvider,
		IJSRuntime jsRuntime,
		IIdentityManager identityManager,
		ITfDataProviderManager dataProviderManager,
		ITfSharedColumnsManager sharedColumnsManager,
		IDataManager dataManager,
		ITfSpaceManager spaceManager,
		NavigationManager navigationManager,
		IToastService toastService,
		IMessageService messageService,
		IStringLocalizer<AppStateUseCase> loc
		)
	{
		_authenticationStateProvider = authenticationStateProvider;
		_jsRuntime = jsRuntime;
		_identityManager = identityManager;
		_dataProviderManager = dataProviderManager;
		_sharedColumnsManager = sharedColumnsManager;
		_dataManager = dataManager;
		_spaceManager = spaceManager;
		_navigationManager = navigationManager;
		_toastService = toastService;
		_messageService = messageService;
		LOC = loc;
	}

	internal bool IsBusy { get; set; } = true;

	internal async Task<TfAppState> InitState(TucUser currentUser, string url, TfAppState result)
	{
		var routeState = _navigationManager.GetRouteState(url);

		result = await InitAdminUsers(currentUser, routeState, result);
		result = await InitSpace(currentUser, routeState, result);
		result = await InitAdminDataProvider(currentUser, routeState, result);
		result = await InitAdminSharedColumns(currentUser, routeState, result);
		result = await InitSpaceView(currentUser, routeState, result);

		return result;
	}


}
