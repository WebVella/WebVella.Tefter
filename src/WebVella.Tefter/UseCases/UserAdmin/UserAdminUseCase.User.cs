namespace WebVella.Tefter.UseCases.UserAdmin;
public partial class UserAdminUseCase
{
	internal async Task<Result<TucUser>> GetUserAsync(Guid userId)
	{
		var userResult = await _identityManager.GetUserAsync(userId);
		if (userResult.IsFailed) return Result.Fail(new Error("GetUserAsync failed").CausedBy(userResult.Errors));
		if (userResult.Value is null) return Result.Ok((TucUser)null);
		return new TucUser(userResult.Value);
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
		if (result.Value is null) return Result.Fail(new Error("SaveUserAsync - no user was created"));
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
		if (result.Value is null) return Result.Fail(new Error("SaveUserAsync - no user was created"));
		return Result.Ok(new TucUser(result.Value));
	}
}
