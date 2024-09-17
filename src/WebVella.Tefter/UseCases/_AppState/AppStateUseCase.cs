using Microsoft.AspNetCore.Localization;

namespace WebVella.Tefter.UseCases.AppStart;

internal partial class AppStateUseCase
{
	private readonly AuthenticationStateProvider _authenticationStateProvider;
	private readonly IJSRuntime _jsRuntime;
	private readonly IIdentityManager _identityManager;
	private readonly ITfDataProviderManager _dataProviderManager;
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
		_spaceManager = spaceManager;
		_navigationManager = navigationManager;
		_toastService = toastService;
		_messageService = messageService;
		LOC = loc;
	}

	internal bool IsBusy { get; set; } = true;

	internal async Task<TfAppState> InitState(TucUser currentUser, string url)
	{
		var result = new TfAppState();
		var routeState = _navigationManager.GetRouteState(url);

		result = await InitAdminUsers(currentUser, routeState,result);
		result = await InitSpace(currentUser, routeState,result);
		result = await InitAdminDataProvider(currentUser, routeState,result);

		return result;
	}


}
