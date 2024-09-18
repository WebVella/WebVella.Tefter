﻿namespace WebVella.Tefter.UseCases.AppStart;
internal partial class AppStateUseCase
{
	internal async Task<TfAppState> InitAdminUsersAsync(TucUser currentUser, TfRouteState routeState, TfAppState result)
	{
		if (
			!(routeState.FirstNode == RouteDataFirstNode.Admin
			&& routeState.SecondNode == RouteDataSecondNode.Users)
			)
		{
			result = result with { AdminUsers = new(), AdminManagedUser = null, UserRoles = new() };
			return result;
		};

		//AdminUsers, AdminUsersPage
		if (
			result.AdminUsers.Count == 0
			|| (routeState.UserId is not null && !result.AdminUsers.Any(x => x.Id == routeState.UserId))
			)
			result = result with { AdminUsers = await GetUsersAsync()};

		//AdminManagedUser, UserRoles
		if (routeState.UserId.HasValue)
		{
			var adminUser = await GetUserAsync(routeState.UserId.Value);
			result = result with { AdminManagedUser = adminUser };
			if (adminUser is not null)
			{
				if (!result.AdminUsers.Any(x => x.Id == adminUser.Id))
				{
					var users = result.AdminUsers.ToList();
					users.Add(adminUser);
					result = result with { AdminUsers = users };
				}

				var roles = await GetUserRolesAsync();
				result = result with { UserRoles = roles ?? new List<TucRole>() };

				//check for the other tabs
				if (routeState.ThirdNode == RouteDataThirdNode.Access)
				{
				}
				else if (routeState.ThirdNode == RouteDataThirdNode.Saves)
				{
				}
			}
		}
		return result;
	}

	internal async Task<List<TucUser>> GetUsersAsync(string search = null, int? page = null, int? pageSize = null)
	{
		var srvResult = await _identityManager.GetUsersAsync();
		if (srvResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetUsersAsync failed").CausedBy(srvResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return new List<TucUser>();
		}

		if (srvResult.Value is null) return new List<TucUser>();
		var records = new List<User>();
		if (!String.IsNullOrWhiteSpace(search))
		{
			var searchProcessed = search.Trim().ToLowerInvariant();
			foreach (var item in srvResult.Value)
			{
				bool hasMatch = false;
				if(item.Email.ToLowerInvariant().Contains(searchProcessed)) hasMatch = true;
				if(item.FirstName.ToLowerInvariant().Contains(searchProcessed)) hasMatch = true;
				if(item.LastName.ToLowerInvariant().Contains(searchProcessed)) hasMatch = true;
				if(hasMatch) records.Add(item);
			}
		}
		else records = srvResult.Value.ToList();

		if (page is null || pageSize is null) return records.Select(x => new TucUser(x)).ToList();

		return records.Skip(RenderUtils.CalcSkip(page.Value,pageSize.Value)).Take(pageSize.Value).Select(x => new TucUser(x)).ToList();

	}
	internal async Task<TucUser> GetUserAsync(Guid userId)
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
	internal async Task<List<TucRole>> GetUserRolesAsync()
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
