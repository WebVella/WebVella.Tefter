
using WebVella.Tefter.Web.Components.FastAccessStateManager;

namespace WebVella.Tefter.UseCases.FastAccess;
public partial class FastAccessUseCase
{
	private readonly IIdentityManager _identityManager;
	private readonly NavigationManager _navigationManager;
	private readonly IDispatcher _dispatcher;
	private readonly IToastService _toastService;
	private readonly IMessageService _messageService;
	private readonly IStringLocalizer<FastAccessUseCase> _loc;

	public FastAccessUseCase(
			IIdentityManager identityManager,
			NavigationManager navigationManager,
			IDispatcher dispatcher,
			IToastService toastService,
			IMessageService messageService,
			IStringLocalizer<FastAccessUseCase> loc
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
		if(type == typeof(TfFastAccessStateManager)) await InitForState();
		else throw new Exception($"Type: {type.Name} not supported in SpaceUseCase");
	}

}
