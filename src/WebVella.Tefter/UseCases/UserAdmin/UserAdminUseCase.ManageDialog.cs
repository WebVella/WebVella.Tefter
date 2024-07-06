namespace WebVella.Tefter.UseCases.UserAdmin;
public partial class UserAdminUseCase
{
	internal TucUserAdminManageForm Form { get; set; }
	internal List<TucRole> AllRoles { get; set; } = new();

	internal async Task InitForManageDialogAsync()
	{
		Form = new TucUserAdminManageForm();
		var rolesResult = await _identityManager.GetRolesAsync();
		if (rolesResult.IsFailed) throw new Exception("GetUserRolesAsync failed");
		if (rolesResult.Value is not null) AllRoles = rolesResult.Value.Select(x => new TucRole(x)).ToList();
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
