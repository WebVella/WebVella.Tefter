namespace WebVella.Tefter.UseCases.AppState;
internal partial class AppStateUseCase
{
	internal async Task<(TfAppState, TfAuxDataState)> InitAdminUsersAsync(IServiceProvider serviceProvider,
		TucUser currentUser,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
	{
		if (
			!(newAppState.Route.HasNode(RouteDataNode.Admin,0)
			&& (newAppState.Route.HasNode(RouteDataNode.Users,1) || newAppState.Route.HasNode(RouteDataNode.Roles,1)))
			)
		{
			newAppState = newAppState with { AdminUsers = new(), AdminManagedUser = null, UserRoles = new() };
			return (newAppState, newAuxDataState);
		}
		;

		//AdminUsers, AdminUsersPage
		if (
			newAppState.AdminUsers.Count == 0
			|| (newAppState.Route.UserId is not null && !newAppState.AdminUsers.Any(x => x.Id == newAppState.Route.UserId))
			)
			newAppState = newAppState with { AdminUsers = await GetUsersAsync() };

		//AdminManagedUser, UserRoles
		var roles = await GetRolesAsync();
		newAppState = newAppState with { UserRoles = roles ?? new List<TucRole>() };
		if (newAppState.Route.UserId.HasValue)
		{
			var adminUser = await GetUserAsync(newAppState.Route.UserId.Value);
			newAppState = newAppState with { AdminManagedUser = adminUser };
			if (adminUser is not null)
			{
				if (!newAppState.AdminUsers.Any(x => x.Id == adminUser.Id))
				{
					var users = newAppState.AdminUsers.ToList();
					users.Add(adminUser);
					newAppState = newAppState with { AdminUsers = users };
				}
			}
		}
		if (newAppState.Route.RoleId.HasValue)
		{
			var adminRole = roles.FirstOrDefault(x => x.Id == newAppState.Route.RoleId.Value);
			newAppState = newAppState with { AdminManagedRole = adminRole };
		}
		return (newAppState, newAuxDataState);
	}

	internal virtual async Task<List<TucUser>> GetUsersAsync(string search = null, int? page = null, int? pageSize = null)
	{
		try
		{
			var users = await _tfService.GetUsersAsync();
			var orderedResults = users.OrderBy(x => x.FirstName).ThenBy(x => x.LastName);
			var records = new List<TfUser>();
			if (!String.IsNullOrWhiteSpace(search))
			{
				var searchProcessed = search.Trim().ToLowerInvariant();
				foreach (var item in orderedResults)
				{
					bool hasMatch = false;
					if (item.Email.ToLowerInvariant().Contains(searchProcessed)) hasMatch = true;
					if (item.FirstName.ToLowerInvariant().Contains(searchProcessed)) hasMatch = true;
					if (item.LastName.ToLowerInvariant().Contains(searchProcessed)) hasMatch = true;
					if (hasMatch) records.Add(item);
				}
			}
			else records = orderedResults.ToList();

			if (page is null || pageSize is null) return records.Select(x => new TucUser(x)).ToList();

			return records.Skip(TfConverters.CalcSkip(page.Value, pageSize.Value)).Take(pageSize.Value).Select(x => new TucUser(x)).ToList();
		}
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceException(
					exception: ex,
					toastErrorMessage: "Unexpected Error",
					toastValidationMessage: "Invalid Data",
					notificationErrorTitle: "Unexpected Error",
					toastService: _toastService,
					messageService: _messageService
				);
			return new List<TucUser>();
		}

	}
	internal virtual async Task<TucUser> GetUserAsync(Guid userId)
	{
		try
		{
			var user = await _tfService.GetUserAsync(userId);

			if (user is null)
				return null;

			return new TucUser(user);
		}
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceException(
					exception: ex,
					toastErrorMessage: "Unexpected Error",
					toastValidationMessage: "Invalid Data",
					notificationErrorTitle: "Unexpected Error",
					toastService: _toastService,
					messageService: _messageService
				);
			return null;
		}
	}
	internal virtual async Task<TucUser> CreateUserWithFormAsync(TucUserAdminManageForm form)
	{
		TfUserBuilder userBuilder = _tfService.CreateUserBuilder(null);
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
			.WithCultureCode(form.Culture.CultureInfo.Name);

		var user = userBuilder.Build();
		user = await _tfService.SaveUserAsync(user);
		return new TucUser(user);
	}

	internal virtual async Task<TucUser> AddRoleToUserAsync(Guid roleId, Guid userId)
	{
		try
		{
			var userSM = await _tfService.GetUserAsync(userId);
			if (userSM is null)
				throw new Exception("User not found");

			var roleSM = await _tfService.GetRoleAsync(roleId);
			if (roleSM is null)
				throw new Exception("Role not found");

			if (userSM.Roles.Any(x => x.Id == roleId))
				return new TucUser(userSM);

			await _tfService.AddUsersRoleAsync(new List<TfUser> { userSM }, roleSM);

			return new TucUser(await _tfService.GetUserAsync(userId));
		}
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceException(
					exception: ex,
					toastErrorMessage: "Unexpected Error",
					toastValidationMessage: "Invalid Data",
					notificationErrorTitle: "Unexpected Error",
					toastService: _toastService,
					messageService: _messageService
				);
			return null;
		}
	}

	internal virtual async Task<TucUser> RemoveRoleFromUserAsync(Guid roleId,Guid userId)
	{
		try
		{
			var userSM = await _tfService.GetUserAsync(userId);
			if (userSM is null)
				throw new Exception("User not found");

			var roleSM = await _tfService.GetRoleAsync(roleId);
			if (roleSM is null)
				throw new Exception("Role not found");

			if (!userSM.Roles.Any(x => x.Id == roleId))
				return new TucUser(userSM);

			await _tfService.RemoveUsersRoleAsync(new List<TfUser> { userSM }, roleSM);

			return new TucUser(await _tfService.GetUserAsync(userId));
		}
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceException(
					exception: ex,
					toastErrorMessage: "Unexpected Error",
					toastValidationMessage: "Invalid Data",
					notificationErrorTitle: "Unexpected Error",
					toastService: _toastService,
					messageService: _messageService
				);
			return null;
		}
	}

	internal virtual async Task<TucUser> UpdateUserWithFormAsync(TucUserAdminManageForm form)
	{
		var currentUser = await _tfService.GetUserAsync(form.Id);
		if (currentUser is null)
			throw new TfException("User does not exist");

		TfUserBuilder userBuilder = _tfService.CreateUserBuilder(currentUser);
		userBuilder
			.WithEmail(form.Email)
			.WithFirstName(form.FirstName)
			.WithLastName(form.LastName)
			.Enabled(form.Enabled)
			.WithThemeMode(form.ThemeMode)
			.WithThemeColor(form.ThemeColor)
			.WithCultureCode(form.Culture.CultureInfo.Name)
			.WithRoles(currentUser.Roles.ToArray());

		if (!String.IsNullOrWhiteSpace(form.Password))
			userBuilder.WithPassword(form.Password);

		var user = userBuilder.Build();
		var result = await _tfService.SaveUserAsync(user);
		return new TucUser(result);
	}
	internal virtual async Task<List<TucRole>> GetRolesAsync()
	{
		try
		{
			var roles = await _tfService.GetRolesAsync();
			return roles.Select(x => new TucRole(x)).ToList();
		}
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceException(
					exception: ex,
					toastErrorMessage: "Unexpected Error",
					toastValidationMessage: "Invalid Data",
					notificationErrorTitle: "Unexpected Error",
					toastService: _toastService,
					messageService: _messageService
				);
			return null;
		}
	}
	internal virtual async Task<List<TucUser>> GetUsersForRoleAsync(Guid roleId)
	{
		try
		{
			var users = await _tfService.GetUsersAsync();
			return users
				.Where(x => x.Roles.Any(r => r.Id == roleId))
				.OrderBy(x => x.Email)
				.Select(x => new TucUser(x))
				.ToList();
		}
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceException(
					exception: ex,
					toastErrorMessage: "Unexpected Error",
					toastValidationMessage: "Invalid Data",
					notificationErrorTitle: "Unexpected Error",
					toastService: _toastService,
					messageService: _messageService
				);
			return null;
		}
	}

	internal virtual async Task<TucRole> CreateRoleWithFormAsync(TucRole form)
	{
		TfRoleBuilder roleBuilder = _tfService.CreateRoleBuilder(null);
		roleBuilder
			.WithName(form.Name)
			.IsSystem(form.IsSystem);

		var role = roleBuilder.Build();
		role = await _tfService.SaveRoleAsync(role);
		return new TucRole(role);
	}

	internal virtual async Task<TucRole> UpdateRoleWithFormAsync(TucRole form)
	{
		var currentRole = await _tfService.GetRoleAsync(form.Id);
		if (currentRole is null)
			throw new TfException("Role does not exist");

		TfRoleBuilder roleBuilder = _tfService.CreateRoleBuilder(currentRole);
		roleBuilder
			.WithName(form.Name)
			.IsSystem(form.IsSystem);

		var role = roleBuilder.Build();
		var result = await _tfService.SaveRoleAsync(role);
		return new TucRole(result);
	}

	internal virtual async Task AddUserToRoleAsync(Guid roleId, Guid userId)
	{
		var currentRole = await _tfService.GetRoleAsync(roleId);
		if (currentRole is null)
			throw new TfException("Role does not exist");

		var user = await _tfService.GetUserAsync(userId);
		if (user is null)
			throw new TfException("User does not exist");

		var roleUsers = await GetUsersForRoleAsync(roleId);
		if (roleUsers.Any(x => x.Id == userId))
			return;
		await _tfService.AddUsersRoleAsync(new List<TfUser> { user }, currentRole);
	}

	internal virtual async Task RemoveUserFromRoleAsync(Guid roleId, Guid userId)
	{
		var currentRole = await _tfService.GetRoleAsync(roleId);
		if (currentRole is null)
			throw new TfException("Role does not exist");

		var user = await _tfService.GetUserAsync(userId);
		if (user is null)
			throw new TfException("User does not exist");

		var roleUsers = await GetUsersForRoleAsync(roleId);
		if (!roleUsers.Any(x => x.Id == userId))
			return;

		await _tfService.RemoveUsersRoleAsync(new List<TfUser> { user }, currentRole);
	}
}
