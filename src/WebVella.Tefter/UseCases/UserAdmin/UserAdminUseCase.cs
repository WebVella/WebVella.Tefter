using WebVella.Tefter.Web.Components.AdminUserDetails;
using WebVella.Tefter.Web.Components.AdminUserDetailsActions;
using WebVella.Tefter.Web.Components.AdminUserDetailsNav;
using WebVella.Tefter.Web.Components.AdminUserNavigation;
using WebVella.Tefter.Web.Components.AdminUserStateManager;
using WebVella.Tefter.Web.Components.UserManageDialog;

namespace WebVella.Tefter.UseCases.UserAdmin;
public partial class UserAdminUseCase
{
	private readonly IIdentityManager _identityManager;
	private readonly NavigationManager _navigationManager;
	private readonly IDispatcher _dispatcher;
	private readonly IToastService _toastService;
	private readonly IMessageService _messageService;
	private readonly IStringLocalizer<UserAdminUseCase> _loc;
	public UserAdminUseCase(
		IIdentityManager identityManager,
		NavigationManager navigationManager,
		IDispatcher dispatcher,
		IToastService toastService,
		IMessageService messageService,
		IStringLocalizer<UserAdminUseCase> loc
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
		if (type == typeof(TfAdminUserDetails)) await InitForDetailsAsync();
		else if (type == typeof(TfAdminUserNavigation)) await InitForNavigationAsync();
		else if (type == typeof(TfAdminUserStateManager)) await InitForStateAsync();
		else if (type == typeof(TfUserManageDialog)) await InitForManageDialogAsync();
		else if (type == typeof(TfAdminUserDetailsActions)) await InitForDetailsActionsAsync();
		else if (type == typeof(TfAdminUserDetailsNav)) await InitForDetailsNavAsync();

		else throw new Exception($"Type: {type.Name} not supported in UserAdminUseCase");

	}


}
