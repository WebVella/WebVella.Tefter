namespace WebVella.Tefter.UseCases.Dashboard;
public partial class DashboardUseCase
{
	private readonly IIdentityManager _identityManager;
	private readonly NavigationManager _navigationManager;
	private readonly IDispatcher _dispatcher;
	private readonly IToastService _toastService;
	private readonly IMessageService _messageService;
	private readonly IStringLocalizer<DashboardUseCase> _loc;

	internal bool IsBusy { get; set; } = false;

	public DashboardUseCase(
			IIdentityManager identityManager,
			NavigationManager navigationManager,
			IDispatcher dispatcher,
			IToastService toastService,
			IMessageService messageService,
			IStringLocalizer<DashboardUseCase> loc
			)
	{
		_identityManager = identityManager;
		_navigationManager = navigationManager;
		_dispatcher = dispatcher;
		_toastService = toastService;
		_messageService = messageService;
		_loc = loc;
	}


	internal async Task Init(Type type)
	{
		if (type == typeof(TfDashboardStateManager)) await InitForState();
		else if (type == typeof(TfDashboard)) await InitDashboard();
		else throw new Exception($"Type: {type.Name} not supported in DashboardUseCase");
	}

}
