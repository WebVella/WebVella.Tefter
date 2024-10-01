using WebVella.Tefter.Api;

namespace WebVella.Tefter.UseCases.AppState;

internal partial class AppStateUseCase
{
	private readonly AuthenticationStateProvider _authenticationStateProvider;
	private readonly IJSRuntime _jsRuntime;
	private readonly IIdentityManager _identityManager;
	private readonly ITfDataProviderManager _dataProviderManager;
	private readonly ITfSharedColumnsManager _sharedColumnsManager;
	private readonly IDataManager _dataManager;
	private readonly ITfSpaceManager _spaceManager;
	private readonly ITfScreenRegionComponentManager _screenRegionComponentManager;
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
		ITfScreenRegionComponentManager screenRegionComponentManager,
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
		_screenRegionComponentManager = screenRegionComponentManager;
		_navigationManager = navigationManager;
		_toastService = toastService;
		_messageService = messageService;
		LOC = loc;
	}

	internal async Task<(TfAppState,TfAuxDataState)> InitState(TucUser currentUser, string url, TfAppState oldAppState,TfAuxDataState oldAuxDataState)
	{
		if (oldAppState == null) oldAppState = new TfAppState();
		if (oldAuxDataState == null) oldAuxDataState = new TfAuxDataState();
		var routeState = _navigationManager.GetRouteState(url);
		var appState = oldAppState with { Hash = oldAppState.Hash };
		var auxDataState = oldAuxDataState with { Hash = oldAuxDataState.Hash };
		appState = await InitAdminUsersAsync(currentUser, routeState, appState,oldAppState, auxDataState, oldAuxDataState);
		appState = await InitSpaceAsync(currentUser, routeState, appState,oldAppState, auxDataState, oldAuxDataState);
		appState = await InitAdminDataProviderAsync(currentUser, routeState, appState,oldAppState, auxDataState, oldAuxDataState);
		appState = await InitAdminSharedColumnsAsync(currentUser, routeState, appState,oldAppState, auxDataState, oldAuxDataState);
		appState = await InitSpaceDataAsync(currentUser, routeState, appState,oldAppState, auxDataState, oldAuxDataState);
		appState = await InitSpaceViewAsync(currentUser, routeState, appState,oldAppState, auxDataState, oldAuxDataState);
		appState = await InitBookmarksAsync(currentUser, routeState, appState,oldAppState, auxDataState, oldAuxDataState);
		appState = await InitHomeAsync(currentUser, routeState, appState,oldAppState, auxDataState, oldAuxDataState);
		appState = await InitPagesAsync(currentUser, routeState, appState,oldAppState, auxDataState, oldAuxDataState);

		return (appState,auxDataState);
	}


}
