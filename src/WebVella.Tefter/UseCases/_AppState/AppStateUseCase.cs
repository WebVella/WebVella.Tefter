namespace WebVella.Tefter.UseCases.AppState;

internal partial class AppStateUseCase
{
	private readonly AuthenticationStateProvider _authenticationStateProvider;
	private readonly IServiceProvider _serviceProvider;
	private readonly IJSRuntime _jsRuntime;
	private readonly IIdentityManager _identityManager;
	private readonly ITfDataProviderManager _dataProviderManager;
	private readonly ITfSharedColumnsManager _sharedColumnsManager;
	private readonly ITfDataManager _dataManager;
	private readonly ITfSpaceManager _spaceManager;
	//private readonly ITfScreenRegionComponentManager _screenRegionComponentManager;
	private readonly ITfMetaProvider _metaProvider;
	private readonly ITfRepositoryService _repositoryManager;
	private readonly NavigationManager _navigationManager;
	private readonly IToastService _toastService;
	private readonly IMessageService _messageService;
	private readonly IStringLocalizer<AppStateUseCase> LOC;


	public AppStateUseCase(
		AuthenticationStateProvider authenticationStateProvider,
		IServiceProvider serviceProvider,
		IJSRuntime jsRuntime,
		IIdentityManager identityManager,
		ITfDataProviderManager dataProviderManager,
		ITfSharedColumnsManager sharedColumnsManager,
		ITfDataManager dataManager,
		ITfSpaceManager spaceManager,
		ITfMetaProvider metaProvider,
		ITfRepositoryService repositoryManager,
		NavigationManager navigationManager,
		IToastService toastService,
		IMessageService messageService,
		IStringLocalizer<AppStateUseCase> loc
		)
	{
		_authenticationStateProvider = authenticationStateProvider;
		_serviceProvider = serviceProvider;
		_jsRuntime = jsRuntime;
		_identityManager = identityManager;
		_dataProviderManager = dataProviderManager;
		_sharedColumnsManager = sharedColumnsManager;
		_dataManager = dataManager;
		_spaceManager = spaceManager;
		_metaProvider = metaProvider;
		_repositoryManager = repositoryManager;
		_navigationManager = navigationManager;
		_toastService = toastService;
		_messageService = messageService;
		LOC = loc;
	}

	internal async Task<(TfAppState, TfAuxDataState)> InitState(TucUser currentUser, string url, TfAppState oldAppState, TfAuxDataState oldAuxDataState)
	{

		if (oldAppState == null) oldAppState = new TfAppState();
		if (oldAuxDataState == null) oldAuxDataState = new TfAuxDataState();
		var route = _navigationManager.GetRouteState(url);
		if (route.Page is null) route = route with { Page = 1 };
		if (route.PageSize is null)
		{
			if (currentUser?.Settings?.PageSize is not null)
				route = route with { PageSize = currentUser.Settings.PageSize };
		}
		if(route.PageSize is null) route = route with { PageSize = TfConstants.PageSize };
		var appState = oldAppState with { Hash = oldAppState.Hash, Route = route, CurrentUser = currentUser };
		var auxDataState = oldAuxDataState with { Hash = oldAuxDataState.Hash };
		(appState, auxDataState) = await InitAdminUsersAsync(_serviceProvider, currentUser, appState, oldAppState, auxDataState, oldAuxDataState);
		(appState, auxDataState) = await InitSpaceAsync(_serviceProvider, currentUser, appState, oldAppState, auxDataState, oldAuxDataState);
		(appState, auxDataState) = await InitAdminDataProviderAsync(_serviceProvider, currentUser, appState, oldAppState, auxDataState, oldAuxDataState);
		(appState, auxDataState) = await InitAdminSharedColumnsAsync(_serviceProvider, currentUser, appState, oldAppState, auxDataState, oldAuxDataState);
		(appState, auxDataState) = await InitAdminFileRepositoryAsync(_serviceProvider, currentUser, appState, oldAppState, auxDataState, oldAuxDataState);
		(appState, auxDataState) = await InitSpaceDataAsync(_serviceProvider, currentUser, appState, oldAppState, auxDataState, oldAuxDataState);
		(appState, auxDataState) = await InitSpaceViewAsync(_serviceProvider, currentUser, appState, oldAppState, auxDataState, oldAuxDataState);
		(appState, auxDataState) = await InitBookmarksAsync(_serviceProvider, currentUser, appState, oldAppState, auxDataState, oldAuxDataState);
		(appState, auxDataState) = await InitHomeAsync(_serviceProvider, currentUser, appState, oldAppState, auxDataState, oldAuxDataState);
		(appState, auxDataState) = await InitPagesAsync(_serviceProvider, currentUser, appState, oldAppState, auxDataState, oldAuxDataState);
		(appState, auxDataState) = await InitSpaceNodeAsync(_serviceProvider, currentUser, appState, oldAppState, auxDataState, oldAuxDataState);

		return (appState, auxDataState);
	}


}
