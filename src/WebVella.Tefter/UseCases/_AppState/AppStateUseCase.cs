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
	private readonly ITfTemplateService _templateService;
	private readonly NavigationManager _navigationManager;
	private readonly IToastService _toastService;
	private readonly IMessageService _messageService;
	private readonly IStringLocalizer<AppStateUseCase> LOC;


	public AppStateUseCase(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
		_authenticationStateProvider = serviceProvider.GetService<AuthenticationStateProvider>();
		_jsRuntime = serviceProvider.GetService<IJSRuntime>();
		_identityManager = serviceProvider.GetService<IIdentityManager>();
		_dataProviderManager = serviceProvider.GetService<ITfDataProviderManager>();
		_sharedColumnsManager = serviceProvider.GetService<ITfSharedColumnsManager>();
		_dataManager = serviceProvider.GetService<ITfDataManager>();
		_spaceManager = serviceProvider.GetService<ITfSpaceManager>();
		_metaProvider = serviceProvider.GetService<ITfMetaProvider>();
		_repositoryManager = serviceProvider.GetService<ITfRepositoryService>();
		_templateService = serviceProvider.GetService<ITfTemplateService>();
		_navigationManager = serviceProvider.GetService<NavigationManager>();
		_toastService = serviceProvider.GetService<IToastService>();
		_messageService = serviceProvider.GetService<IMessageService>();
		LOC = serviceProvider.GetService<IStringLocalizer<AppStateUseCase>>();
	}

	internal virtual async Task<(TfAppState, TfAuxDataState)> InitState(TucUser currentUser, string url, TfAppState oldAppState, TfAuxDataState oldAuxDataState)
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
		if (route.PageSize is null) route = route with { PageSize = TfConstants.PageSize };
		var appState = oldAppState with { Hash = oldAppState.Hash, Route = route, CurrentUser = currentUser };
		var auxDataState = oldAuxDataState with { Hash = oldAuxDataState.Hash };
		(appState, auxDataState) = await InitAdminUsersAsync(_serviceProvider, currentUser, appState, oldAppState, auxDataState, oldAuxDataState);
		(appState, auxDataState) = await InitAdminDataProviderAsync(_serviceProvider, currentUser, appState, oldAppState, auxDataState, oldAuxDataState);
		(appState, auxDataState) = await InitAdminSharedColumnsAsync(_serviceProvider, currentUser, appState, oldAppState, auxDataState, oldAuxDataState);
		(appState, auxDataState) = await InitAdminFileRepositoryAsync(_serviceProvider, currentUser, appState, oldAppState, auxDataState, oldAuxDataState);
		(appState, auxDataState) = await InitAdminTemplatesAsync(_serviceProvider, currentUser, appState, oldAppState, auxDataState, oldAuxDataState);
		(appState, auxDataState) = await InitBookmarksAsync(_serviceProvider, currentUser, appState, oldAppState, auxDataState, oldAuxDataState);
		(appState, auxDataState) = await InitHomeAsync(_serviceProvider, currentUser, appState, oldAppState, auxDataState, oldAuxDataState);
		(appState, auxDataState) = await InitPagesAsync(_serviceProvider, currentUser, appState, oldAppState, auxDataState, oldAuxDataState);
		(appState, auxDataState) = await InitSpaceAsync(_serviceProvider, currentUser, appState, oldAppState, auxDataState, oldAuxDataState);
		(appState, auxDataState) = await InitSpaceNodeAsync(_serviceProvider, currentUser, appState, oldAppState, auxDataState, oldAuxDataState);
		(appState, auxDataState) = await InitSpaceDataAsync(_serviceProvider, currentUser, appState, oldAppState, auxDataState, oldAuxDataState);
		(appState, auxDataState) = await InitSpaceViewAsync(_serviceProvider, currentUser, appState, oldAppState, auxDataState, oldAuxDataState);
		return (appState, auxDataState);
	}


}
