﻿namespace WebVella.Tefter.UseCases.AppStart;
internal partial class AppStateUseCase
{
	public async Task<List<TucUser>> GetUsersAsync(string search = null, int? page = null, int? pageSize = null)
	{
		var userResult = await _identityManager.GetUsersAsync(search: search, page: page, pageSize: pageSize);
		if (userResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetUsersAsync failed").CausedBy(userResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return new List<TucUser>();
		}

		if (userResult.Value is null) return new List<TucUser>();

		return userResult.Value.Select(x => new TucUser(x)).ToList();
	}
	public async Task<TucUser> GetUser(Guid userId)
	{
		var srvResult = await _identityManager.GetUserAsync(userId);
		if (srvResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetUserAsync failed").CausedBy(srvResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		if (srvResult.Value is null) return null;
		return new TucUser(srvResult.Value);
	}
	public async Task<List<TucRole>> GetUserRoles()
	{
		var srvResult = await _identityManager.GetRolesAsync();
		if (srvResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetRolesAsync failed").CausedBy(srvResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		if (srvResult.Value is null) return null;
		return srvResult.Value.Select(x => new TucRole(x)).ToList();
	}
	internal async Task<Result<TucUser>> CreateUserWithFormAsync(TucUserAdminManageForm form)
	{
		UserBuilder userBuilder = _identityManager.CreateUserBuilder(null);
		userBuilder
			.WithEmail(form.Email)
			.WithFirstName(form.FirstName)
			.WithLastName(form.LastName)
			.WithPassword(form.Password)
			.Enabled(form.Enabled)
			.CreatedOn(DateTime.Now)
			.WithThemeMode(form.ThemeMode)
			.WithThemeColor(form.ThemeColor)
			.WithOpenSidebar(true)
			.WithCultureCode(form.Culture.CultureInfo.Name)
			.WithRoles(form.Roles.Select(x => x.ToModel()).ToArray());

		var user = userBuilder.Build();
		var result = await _identityManager.SaveUserAsync(user);
		if (result.IsFailed) return Result.Fail(new Error("SaveUserAsync failed").CausedBy(result.Errors));

		return Result.Ok(new TucUser(result.Value));
	}

	internal async Task<Result<TucUser>> UpdateUserWithFormAsync(TucUserAdminManageForm form)
	{
		var currentUserResult = await _identityManager.GetUserAsync(form.Id);
		if (currentUserResult.IsFailed) return Result.Fail(new Error("GetUserAsync failed").CausedBy(currentUserResult.Errors));
		if (currentUserResult.Value is null) return Result.Fail(new Error("GetUserAsync - no user was created"));

		UserBuilder userBuilder = _identityManager.CreateUserBuilder(currentUserResult.Value);
		userBuilder
			.WithEmail(form.Email)
			.WithFirstName(form.FirstName)
			.WithLastName(form.LastName)
			.Enabled(form.Enabled)
			.WithThemeMode(form.ThemeMode)
			.WithThemeColor(form.ThemeColor)
			.WithCultureCode(form.Culture.CultureInfo.Name)
			.WithRoles(form.Roles.Select(x => x.ToModel()).ToArray());

		if (!String.IsNullOrWhiteSpace(form.Password))
			userBuilder.WithPassword(form.Password);

		var user = userBuilder.Build();
		var result = await _identityManager.SaveUserAsync(user);
		if (result.IsFailed) return Result.Fail(new Error("SaveUserAsync failed").CausedBy(result.Errors));
		return Result.Ok(new TucUser(result.Value));
	}
}
