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


	internal async Task<Result<TucUser>> CreateUserWithFormAsync()
	{
		UserBuilder userBuilder = _identityManager.CreateUserBuilder(null);
		userBuilder
			.WithEmail(Form.Email)
			.WithFirstName(Form.FirstName)
			.WithLastName(Form.LastName)
			.WithPassword(Form.Password)
			.Enabled(Form.Enabled)
			.CreatedOn(DateTime.Now)
			.WithThemeMode(Form.ThemeMode)
			.WithThemeColor(Form.ThemeColor)
			.WithOpenSidebar(true)
			.WithCultureCode(Form.Culture.CultureInfo.Name)
			.WithRoles(Form.Roles.Select(x => x.ToModel()).ToArray());

		var user = userBuilder.Build();
		var result = await _identityManager.SaveUserAsync(user);
		if (result.IsFailed) return Result.Fail(new Error("SaveUserAsync failed").CausedBy(result.Errors));
		
		return Result.Ok(new TucUser(result.Value));
	}

	internal async Task<Result<TucUser>> UpdateUserWithFormAsync()
	{
		var currentUserResult = await _identityManager.GetUserAsync(Form.Id);
		if (currentUserResult.IsFailed) return Result.Fail(new Error("GetUserAsync failed").CausedBy(currentUserResult.Errors));
		if (currentUserResult.Value is null) return Result.Fail(new Error("GetUserAsync - no user was created"));

		UserBuilder userBuilder = _identityManager.CreateUserBuilder(currentUserResult.Value);
		userBuilder
			.WithEmail(Form.Email)
			.WithFirstName(Form.FirstName)
			.WithLastName(Form.LastName)
			.Enabled(Form.Enabled)
			.WithThemeMode(Form.ThemeMode)
			.WithThemeColor(Form.ThemeColor)
			.WithCultureCode(Form.Culture.CultureInfo.Name)
			.WithRoles(Form.Roles.Select(x => x.ToModel()).ToArray());

		if (!String.IsNullOrWhiteSpace(Form.Password))
			userBuilder.WithPassword(Form.Password);

		var user = userBuilder.Build();
		var result = await _identityManager.SaveUserAsync(user);
		if (result.IsFailed) return Result.Fail(new Error("SaveUserAsync failed").CausedBy(result.Errors));
		return Result.Ok(new TucUser(result.Value));
	}
}
