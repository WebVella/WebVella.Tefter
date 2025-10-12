using Nito.AsyncEx.Synchronous;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	/// <summary>
	/// Creates a new instance of <see cref="TfUserBuilder"/> for building users.
	/// </summary>
	/// <param name="user">Optional user to initialize the builder with.</param>
	/// <returns>A <see cref="TfUserBuilder"/> instance.</returns>
	TfUserBuilder CreateUserBuilder(
		TfUser user = null);

	/// <summary>
	/// Retrieves a user by their unique identifier.
	/// </summary>
	/// <param name="id">The unique identifier of the user.</param>
	/// <returns>The <see cref="TfUser"/> instance if found; otherwise, null.</returns>
	TfUser GetUser(
		Guid id);

	/// <summary>
	/// Retrieves a user by their email address.
	/// </summary>
	/// <param name="email">The email address of the user.</param>
	/// <returns>The <see cref="TfUser"/> instance if found; otherwise, null.</returns>
	TfUser GetUser(
		string email);

	/// <summary>
	/// Retrieves a user for the email sender service
	/// </summary>
	/// <returns>The <see cref="TfUser"/> instance if found; otherwise, null.</returns>
	TfUser GetDefaultSystemUser();

	/// <summary>
	/// Retrieves a user by their email address and password.
	/// </summary>
	/// <param name="email">The email address of the user.</param>
	/// <param name="password">The password of the user.</param>
	/// <returns>The <see cref="TfUser"/> instance if found; otherwise, null.</returns>
	TfUser GetUser(
		string email,
		string password);

	/// <summary>
	/// Retrieves all users in the system.
	/// </summary>
	/// <returns>A read-only collection of <see cref="TfUser"/> instances.</returns>
	ReadOnlyCollection<TfUser> GetUsers();

	/// <summary>
	/// Retrieves all users in the system matching the search string.
	/// </summary>
	/// <returns>A read-only collection of <see cref="TfUser"/> instances.</returns>
	ReadOnlyCollection<TfUser> GetUsers(string? search);

	ReadOnlyCollection<TfUser> GetUsersForRole(Guid roleId);

	/// <summary>
	/// Saves a user to the system. Creates a new user if it does not exist, or updates an existing one.
	/// </summary>
	/// <param name="user">The user to save.</param>
	/// <returns>The saved <see cref="TfUser"/> instance.</returns>
	TfUser SaveUser(
		TfUser user);

	/// <summary>
	/// Creates user
	/// </summary>
	/// <param name="user"></param>
	/// <returns></returns>
	public TfUser CreateUser(
		TfUser user);

	/// <summary>
	/// Updates existing user
	/// </summary>
	/// <param name="user"></param>
	/// <returns></returns>
	public TfUser UpdateUser(
		TfUser user);

	/// <summary>
	/// Asynchronously retrieves a user by their unique identifier.
	/// </summary>
	/// <param name="id">The unique identifier of the user.</param>
	/// <returns>A task representing the asynchronous operation, with the <see cref="TfUser"/> instance if found; otherwise, null.</returns>
	Task<TfUser> GetUserAsync(
		Guid id);

	/// <summary>
	/// Asynchronously retrieves a user by their email address.
	/// </summary>
	/// <param name="email">The email address of the user.</param>
	/// <returns>A task representing the asynchronous operation, with the <see cref="TfUser"/> instance if found; otherwise, null.</returns>
	Task<TfUser> GetUserAsync(
		string email);

	/// <summary>
	/// Asynchronously retrieves a user by their email address and password.
	/// </summary>
	/// <param name="email">The email address of the user.</param>
	/// <param name="password">The password of the user.</param>
	/// <returns>A task representing the asynchronous operation, with the <see cref="TfUser"/> instance if found; otherwise, null.</returns>
	Task<TfUser> GetUserAsync(
		string email,
		string password);

	/// <summary>
	/// Asynchronously retrieves all users in the system.
	/// </summary>
	/// <returns>A task representing the asynchronous operation, with a read-only collection of <see cref="TfUser"/> instances.</returns>
	Task<ReadOnlyCollection<TfUser>> GetUsersAsync();

	/// <summary>
	/// Asynchronously saves a user to the system. Creates a new user if it does not exist, or updates an existing one.
	/// </summary>
	/// <param name="user">The user to save.</param>
	/// <returns>A task representing the asynchronous operation, with the saved <see cref="TfUser"/> instance.</returns>
	Task<TfUser> SaveUserAsync(
		TfUser user);

	/// <summary>
	/// Creates user
	/// </summary>
	/// <param name="user"></param>
	/// <returns></returns>
	Task<TfUser> CreateUserAsync(
		TfUser user);

	public Task<TfUser> CreateUserWithFormAsync(TfUserManageForm form);

	/// <summary>
	/// Updates user
	/// </summary>
	/// <param name="user"></param>
	/// <returns></returns>
	Task<TfUser> UpdateUserAsync(
		TfUser user);

	Task<TfUser> UpdateUserWithFormAsync(TfUserManageForm form);
	Task<TfUser> UpdateUserVisualPreferencesAsync(Guid userId, DesignThemeModes themeMode, TfCultureOption culture);

	//Gets the authenticated user from cookie
	Task<TfUser?> GetUserFromCookieAsync(IJSRuntime jsRuntime, AuthenticationStateProvider authStateProvider);

	//Checks user current URL access
	bool UserHasAccess(TfUser user, NavigationManager navigator, string? urlOverride = null);

	/// <summary>
	/// Removes a role from a list of users.
	/// </summary>
	/// <param name="users">The list of users to remove the role from.</param>
	/// <param name="role">The role to remove.</param>
	void RemoveUsersRole(
		List<TfUser> users,
		TfRole role);


	Task<TfUser> RemoveUserFromRoleAsync(
		Guid userId,
		Guid roleId);

	/// <summary>
	/// Asynchronously removes a role from a list of users.
	/// </summary>
	/// <param name="users">The list of users to remove the role from.</param>
	/// <param name="role">The role to remove.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	Task RemoveUsersRoleAsync(
		List<TfUser> users,
		TfRole role);

	/// <summary>
	/// Adds a role to a list of users.
	/// </summary>
	/// <param name="users">The list of users to add the role to.</param>
	/// <param name="role">The role to add.</param>
	void AddUsersRole(
		List<TfUser> users,
		TfRole role);

	Task<TfUser> AddUserToRoleAsync(
		Guid userId,
		Guid roleId);

	/// <summary>
	/// Asynchronously adds a role to a list of users.
	/// </summary>
	/// <param name="users">The list of users to add the role to.</param>
	/// <param name="role">The role to add.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	Task AddUsersRoleAsync(
		List<TfUser> users,
		TfRole role);

	Task<TfUser> SetStartUpUrl(Guid userId, string? url);

	Task<TfUser> SetUserCulture(Guid userId, string cultureCode);

	Task<TfUser> SetPageSize(Guid userId, int? pageSize);

	Task<TfUser> SetViewPresetColumnPersonalization(Guid userId, Guid spaceViewId, Guid? presetId,
		Guid spaceViewColumnId, short? width);

	List<TfSortQuery> CalculateViewPresetSortPersonalization(List<TfSortQuery> currentSorts, Guid spaceViewId,
		Guid spaceViewColumnId, bool hasShiftKey);

	Task<TfUser> RemoveSpaceViewPersonalizations(Guid userId, Guid spaceViewId, Guid? presetId);
}

public partial class TfService : ITfService
{
	public TfUserBuilder CreateUserBuilder(
		TfUser user = null)
	{
		try
		{
			return new TfUserBuilder(this, user);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfUser GetDefaultSystemUser()
	{
		try
		{
			var users = GetUsers();
			var adminUsers = users.Where(x => x.Roles.Any(r => r.Id == TfConstants.ADMIN_ROLE_ID)).ToList();
			if (adminUsers.Count > 0)
				return adminUsers.First();

			if (users.Count > 0)
				return users.First();

			return null;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfUser GetUser(
		Guid id)
	{
		try
		{
			var userDbo = _dboManager.Get<UserDbo>(id);
			if (userDbo == null)
				return null;

			var roles = GetRoles();

			var userRoles = new List<TfRole>();
			foreach (var userRoleRelation in _dboManager.GetList<UserRoleDbo>(id, nameof(UserRoleDbo.UserId)))
				userRoles.Add(roles.Single(x => x.Id == userRoleRelation.RoleId));

			var userBuilder = new TfUserBuilder(this, userDbo.Id)
				.WithEmail(userDbo.Email)
				.WithFirstName(userDbo.FirstName)
				.WithLastName(userDbo.LastName)
				.CreatedOn(userDbo.CreatedOn)
				.Enabled(userDbo.Enabled)
				.WithPassword(userDbo.Password)
				.WithRoles(userRoles.ToArray());

			if (userDbo.Settings != null)
			{
				userBuilder
					.WithThemeMode(userDbo.Settings.ThemeMode)
					.WithOpenSidebar(userDbo.Settings.IsSidebarOpen)
					.WithCultureCode(userDbo.Settings.CultureName)
					.WithStartUpUrl(userDbo.Settings.StartUpUrl)
					.WithPageSize(userDbo.Settings.PageSize)
					.WithViewPresetColumnPersonalizations(userDbo.Settings.ViewPresetColumnPersonalizations)
					.WithViewPresetSortPersonalizations(userDbo.Settings.ViewPresetSortPersonalizations);
			}

			return userBuilder.Build();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfUser GetUser(
		string email)
	{
		try
		{
			var valEx = new TfValidationException();

			if (email == null)
				valEx.AddValidationError(nameof(TfUser.Email), "The email is required.");

			valEx.ThrowIfContainsErrors();

			var userDbo = _dboManager.Get<UserDbo>(email, nameof(UserDbo.Email));
			if (userDbo == null)
				return null;

			var roles = GetRoles();
			var userRoles = new List<TfRole>();
			foreach (var userRoleRelation in _dboManager.GetList<UserRoleDbo>(userDbo.Id, nameof(UserRoleDbo.UserId)))
				userRoles.Add(roles.Single(x => x.Id == userRoleRelation.RoleId));

			var userBuilder = new TfUserBuilder(this, userDbo.Id)
				.WithEmail(userDbo.Email)
				.WithFirstName(userDbo.FirstName)
				.WithLastName(userDbo.LastName)
				.CreatedOn(userDbo.CreatedOn)
				.Enabled(userDbo.Enabled)
				.WithPassword(userDbo.Password)
				.WithRoles(userRoles.ToArray());

			if (userDbo.Settings != null)
			{
				userBuilder
					.WithThemeMode(userDbo.Settings.ThemeMode)
					.WithOpenSidebar(userDbo.Settings.IsSidebarOpen)
					.WithCultureCode(userDbo.Settings.CultureName)
					.WithStartUpUrl(userDbo.Settings.StartUpUrl)
					.WithPageSize(userDbo.Settings.PageSize)
					.WithViewPresetColumnPersonalizations(userDbo.Settings.ViewPresetColumnPersonalizations)
					.WithViewPresetSortPersonalizations(userDbo.Settings.ViewPresetSortPersonalizations);
			}

			return userBuilder.Build();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfUser GetUser(
		string email,
		string password)
	{
		try
		{
			var valEx = new TfValidationException();

			if (email == null)
				valEx.AddValidationError(nameof(TfUser.Email), "The email is required.");

			if (password == null)
				valEx.AddValidationError(nameof(TfUser.Password), "The password is required.");

			valEx.ThrowIfContainsErrors();


			var userDbo = _dboManager.Get<UserDbo>(email, nameof(UserDbo.Email));
			if (userDbo == null)
				return null;

			if (userDbo.Password != password.ToMD5Hash())
				return null;

			var roles = GetRoles();
			var userRoles = new List<TfRole>();
			foreach (var userRoleRelation in _dboManager.GetList<UserRoleDbo>(userDbo.Id, nameof(UserRoleDbo.UserId)))
				userRoles.Add(roles.Single(x => x.Id == userRoleRelation.RoleId));

			var userBuilder = new TfUserBuilder(this, userDbo.Id)
				.WithEmail(userDbo.Email)
				.WithFirstName(userDbo.FirstName)
				.WithLastName(userDbo.LastName)
				.CreatedOn(userDbo.CreatedOn)
				.Enabled(userDbo.Enabled)
				.WithPassword(userDbo.Password)
				.WithRoles(userRoles.ToArray());

			if (userDbo.Settings != null)
			{
				userBuilder
					.WithThemeMode(userDbo.Settings.ThemeMode)
					.WithOpenSidebar(userDbo.Settings.IsSidebarOpen)
					.WithCultureCode(userDbo.Settings.CultureName)
					.WithStartUpUrl(userDbo.Settings.StartUpUrl)
					.WithPageSize(userDbo.Settings.PageSize)
					.WithViewPresetColumnPersonalizations(userDbo.Settings.ViewPresetColumnPersonalizations)
					.WithViewPresetSortPersonalizations(userDbo.Settings.ViewPresetSortPersonalizations);
			}

			return userBuilder.Build();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public ReadOnlyCollection<TfUser> GetUsers()
	{
		try
		{
			var orderSettings = new TfOrderSettings(nameof(UserDbo.LastName), OrderDirection.ASC)
				.Add(nameof(UserDbo.FirstName), OrderDirection.ASC);

			var usersDbo = _dboManager.GetList<UserDbo>(order: orderSettings);

			var roles = GetRoles();
			var userRolesDict = new Dictionary<Guid, List<TfRole>>();
			foreach (var userRoleRelation in _dboManager.GetList<UserRoleDbo>())
			{
				if (!userRolesDict.ContainsKey(userRoleRelation.UserId))
					userRolesDict[userRoleRelation.UserId] = new List<TfRole>();

				userRolesDict[userRoleRelation.UserId].Add(roles.Single(x => x.Id == userRoleRelation.RoleId));
			}

			List<TfUser> users = new List<TfUser>();
			foreach (var userDbo in usersDbo)
			{
				var userBuilder = new TfUserBuilder(this, userDbo.Id)
					.WithEmail(userDbo.Email)
					.WithFirstName(userDbo.FirstName)
					.WithLastName(userDbo.LastName)
					.CreatedOn(userDbo.CreatedOn)
					.Enabled(userDbo.Enabled)
					.WithPassword(userDbo.Password);

				if (userDbo.Settings != null)
				{
					userBuilder
						.WithThemeMode(userDbo.Settings.ThemeMode)
						.WithOpenSidebar(userDbo.Settings.IsSidebarOpen)
						.WithCultureCode(userDbo.Settings.CultureName)
						.WithStartUpUrl(userDbo.Settings.StartUpUrl)
						.WithPageSize(userDbo.Settings.PageSize)
						.WithViewPresetColumnPersonalizations(userDbo.Settings.ViewPresetColumnPersonalizations)
						.WithViewPresetSortPersonalizations(userDbo.Settings.ViewPresetSortPersonalizations);
				}

				if (userRolesDict.ContainsKey(userDbo.Id))
					userBuilder.WithRoles(userRolesDict[userDbo.Id].ToArray());

				users.Add(userBuilder.Build());
			}

			return users.AsReadOnly();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public ReadOnlyCollection<TfUser> GetUsers(string? search)
	{
		try
		{
			var allUsers = GetUsers();
			if (String.IsNullOrWhiteSpace(search))
				return allUsers;

			search = search.Trim().ToLowerInvariant();
			return allUsers.Where(x =>
				x.Email.ToLowerInvariant().Contains(search)
				|| x.FirstName.ToLowerInvariant().Contains(search)
				|| x.LastName.ToLowerInvariant().Contains(search)
			).ToList().AsReadOnly();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public ReadOnlyCollection<TfUser> GetUsersForRole(Guid roleId)
	{
		try
		{
			var allUsers = GetUsers();

			return allUsers
				.Where(x => x.Roles.Any(r => r.Id == roleId))
				.OrderBy(x => x.Email)
				.ToList().AsReadOnly();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfUser SaveUser(
		TfUser user)
	{
		try
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (user.Id == Guid.Empty)
				return CreateUser(user);
			else
			{
				var existingUser = GetUser(user.Id);
				if (existingUser is not null)
					return UpdateUser(user);
				else
					return CreateUser(user);
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfUser CreateUser(
		TfUser user)
	{
		new UserValidator(this)
			.ValidateCreate(user)
			.ToValidationException()
			.ThrowIfContainsErrors();

		UserDbo userDbo = new UserDbo
		{
			Id = user.Id == Guid.Empty ? Guid.NewGuid() : user.Id,
			CreatedOn = user.CreatedOn,
			Email = user.Email,
			Enabled = user.Enabled,
			FirstName = user.FirstName,
			LastName = user.LastName,
			Password = user.Password.ToMD5Hash(),
			Settings = user.Settings,
			XSearch = $"{user.FirstName} {user.LastName} {user.Email}"
		};

		using (TfDatabaseTransactionScope scope = _dbService.CreateTransactionScope())
		{
			bool success = _dboManager.Insert<UserDbo>(userDbo);

			if (!success)
				throw new TfDboServiceException("Insert<UserDbo> failed");

			foreach (var role in user.Roles)
			{
				var dbo = new UserRoleDbo { RoleId = role.Id, UserId = userDbo.Id };

				success = _dboManager.Insert<UserRoleDbo>(dbo);
				if (!success)
					throw new TfDboServiceException("Insert<UserRoleDbo> failed");
			}

			scope.Complete();

			PublishEventWithScope(new TfUserCreatedEvent(GetUser(userDbo.Id)));
		}

		return GetUser(userDbo.Id);
	}

	public TfUser UpdateUser(
		TfUser user)
	{
		new UserValidator(this)
			.ValidateUpdate(user)
			.ToValidationException()
			.ThrowIfContainsErrors();

		//if password is changed, hash new password
		var existingUser = GetUser(user.Id);
		string password = user.Password;
		if (existingUser.Password != user.Password)
			password = user.Password.ToMD5Hash();

		UserDbo userDbo = new UserDbo
		{
			Id = Guid.NewGuid(),
			CreatedOn = user.CreatedOn,
			Email = user.Email,
			Enabled = user.Enabled,
			FirstName = user.FirstName,
			LastName = user.LastName,
			Password = password,
			Settings = user.Settings,
			XSearch = $"{user.FirstName} {user.LastName} {user.Email}"
		};

		using (TfDatabaseTransactionScope scope = _dbService.CreateTransactionScope())
		{
			//update user info
			bool success = _dboManager.Update<UserDbo>(userDbo);

			if (!success)
				throw new TfDboServiceException("Update<UserDbo> failed");

			//remove old roles
			foreach (var role in existingUser.Roles)
			{
				var dbId = new Dictionary<string, Guid>
				{
					{ nameof(UserRoleDbo.UserId), user.Id }, { nameof(UserRoleDbo.RoleId), role.Id }
				};

				success = _dboManager.Delete<UserRoleDbo>(dbId);

				if (!success)
					throw new TfDboServiceException("Delete<UserRoleDbo> failed");
			}

			//add new roles
			foreach (var role in user.Roles)
			{
				var dbo = new UserRoleDbo { RoleId = role.Id, UserId = userDbo.Id };

				success = _dboManager.Insert<UserRoleDbo>(dbo);

				if (!success)
					throw new TfDboServiceException("Insert<UserRoleDbo> failed");
			}

			scope.Complete();

			PublishEventWithScope(new TfUserCreatedEvent(GetUser(userDbo.Id)));
		}

		return GetUser(userDbo.Id);
	}

	public void AddUsersRole(
		List<TfUser> users,
		TfRole role)
	{
		try
		{
			if (users == null)
				throw new ArgumentNullException(nameof(users));
			if (role == null)
				throw new ArgumentNullException(nameof(role));

			using (TfDatabaseTransactionScope scope = _dbService.CreateTransactionScope())
			{
				foreach (var user in users)
				{
					var existingUser = GetUser(user.Id);
					if (existingUser is null)
						throw new TfValidationException($"User with id {user.Id} does not exist.");

					if (existingUser.Roles.Any(x => x.Id == role.Id))
						continue;

					var success = _dboManager.Insert<UserRoleDbo>(
						new UserRoleDbo { RoleId = role.Id, UserId = user.Id });

					if (!success)
						throw new TfDboServiceException("Insert<UserRoleDbo> failed");
				}

				scope.Complete();
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void RemoveUsersRole(
		List<TfUser> users,
		TfRole role)
	{
		try
		{
			if (users == null)
				throw new ArgumentNullException(nameof(users));
			if (role == null)
				throw new ArgumentNullException(nameof(role));

			using (TfDatabaseTransactionScope scope = _dbService.CreateTransactionScope())
			{
				foreach (var user in users)
				{
					var existingUser = GetUser(user.Id);
					if (existingUser is null)
						throw new TfValidationException($"User with id {user.Id} does not exist.");

					if (!existingUser.Roles.Any(x => x.Id == role.Id))
						continue;

					var dbId = new Dictionary<string, Guid>
					{
						{ nameof(UserRoleDbo.UserId), user.Id }, { nameof(UserRoleDbo.RoleId), role.Id }
					};

					var success = _dboManager.Delete<UserRoleDbo>(dbId);

					if (!success)
						throw new TfDboServiceException("Delete<UserRoleDbo> failed");
				}

				scope.Complete();
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public async Task<TfUser> GetUserAsync(
		Guid id)
	{
		try
		{
			var userDbo = await _dboManager.GetAsync<UserDbo>(id);
			if (userDbo == null)
				return null;

			var roles = await GetRolesAsync();
			var userRoles = new List<TfRole>();
			foreach (var userRoleRelation in
			         await _dboManager.GetListAsync<UserRoleDbo>(id, nameof(UserRoleDbo.UserId)))
				userRoles.Add(roles.Single(x => x.Id == userRoleRelation.RoleId));

			var userBuilder = new TfUserBuilder(this, userDbo.Id)
				.WithEmail(userDbo.Email)
				.WithFirstName(userDbo.FirstName)
				.WithLastName(userDbo.LastName)
				.CreatedOn(userDbo.CreatedOn)
				.Enabled(userDbo.Enabled)
				.WithPassword(userDbo.Password)
				.WithRoles(userRoles.ToArray());

			if (userDbo.Settings != null)
			{
				userBuilder
					.WithThemeMode(userDbo.Settings.ThemeMode)
					.WithOpenSidebar(userDbo.Settings.IsSidebarOpen)
					.WithCultureCode(userDbo.Settings.CultureName)
					.WithStartUpUrl(userDbo.Settings.StartUpUrl)
					.WithPageSize(userDbo.Settings.PageSize)
					.WithViewPresetColumnPersonalizations(userDbo.Settings.ViewPresetColumnPersonalizations)
					.WithViewPresetSortPersonalizations(userDbo.Settings.ViewPresetSortPersonalizations);
			}

			return userBuilder.Build();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public async Task<TfUser> GetUserAsync(
		string email)
	{
		try
		{
			var valEx = new TfValidationException();

			if (email == null)
				valEx.AddValidationError(nameof(TfUser.Email), "The email is required.");

			valEx.ThrowIfContainsErrors();

			var userDbo = await _dboManager.GetAsync<UserDbo>(email, nameof(UserDbo.Email));
			if (userDbo == null)
				return null;

			var roles = await GetRolesAsync();
			var userRoles = new List<TfRole>();
			foreach (var userRoleRelation in await _dboManager.GetListAsync<UserRoleDbo>(userDbo.Id,
				         nameof(UserRoleDbo.UserId)))
				userRoles.Add(roles.Single(x => x.Id == userRoleRelation.RoleId));

			var userBuilder = new TfUserBuilder(this, userDbo.Id)
				.WithEmail(userDbo.Email)
				.WithFirstName(userDbo.FirstName)
				.WithLastName(userDbo.LastName)
				.CreatedOn(userDbo.CreatedOn)
				.Enabled(userDbo.Enabled)
				.WithPassword(userDbo.Password)
				.WithRoles(userRoles.ToArray());

			if (userDbo.Settings != null)
			{
				userBuilder
					.WithThemeMode(userDbo.Settings.ThemeMode)
					.WithOpenSidebar(userDbo.Settings.IsSidebarOpen)
					.WithCultureCode(userDbo.Settings.CultureName)
					.WithStartUpUrl(userDbo.Settings.StartUpUrl)
					.WithPageSize(userDbo.Settings.PageSize)
					.WithViewPresetColumnPersonalizations(userDbo.Settings.ViewPresetColumnPersonalizations)
					.WithViewPresetSortPersonalizations(userDbo.Settings.ViewPresetSortPersonalizations);
			}

			return userBuilder.Build();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public async Task<TfUser> GetUserAsync(
		string email,
		string password)
	{
		try
		{
			var valEx = new TfValidationException();

			if (email == null)
				valEx.AddValidationError(nameof(TfUser.Email), "The email is required.");

			if (string.IsNullOrWhiteSpace(password))
				valEx.AddValidationError(nameof(TfUser.Password), "The password is required.");

			valEx.ThrowIfContainsErrors();

			var userDbo = await _dboManager.GetAsync<UserDbo>(email, nameof(UserDbo.Email));
			if (userDbo == null)
				return null;

			if (userDbo.Password != password.ToMD5Hash())
				return null;


			var roles = await GetRolesAsync();
			var userRoles = new List<TfRole>();
			foreach (var userRoleRelation in await _dboManager.GetListAsync<UserRoleDbo>(userDbo.Id,
				         nameof(UserRoleDbo.UserId)))
				userRoles.Add(roles.Single(x => x.Id == userRoleRelation.RoleId));

			var userBuilder = new TfUserBuilder(this, userDbo.Id)
				.WithEmail(userDbo.Email)
				.WithFirstName(userDbo.FirstName)
				.WithLastName(userDbo.LastName)
				.CreatedOn(userDbo.CreatedOn)
				.Enabled(userDbo.Enabled)
				.WithPassword(userDbo.Password)
				.WithRoles(userRoles.ToArray());

			if (userDbo.Settings != null)
			{
				userBuilder
					.WithThemeMode(userDbo.Settings.ThemeMode)
					.WithOpenSidebar(userDbo.Settings.IsSidebarOpen)
					.WithCultureCode(userDbo.Settings.CultureName)
					.WithStartUpUrl(userDbo.Settings.StartUpUrl)
					.WithPageSize(userDbo.Settings.PageSize)
					.WithViewPresetColumnPersonalizations(userDbo.Settings.ViewPresetColumnPersonalizations)
					.WithViewPresetSortPersonalizations(userDbo.Settings.ViewPresetSortPersonalizations);
			}

			return userBuilder.Build();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public async Task<ReadOnlyCollection<TfUser>> GetUsersAsync()
	{
		try
		{
			var orderSettings = new TfOrderSettings(nameof(UserDbo.LastName), OrderDirection.ASC)
				.Add(nameof(UserDbo.FirstName), OrderDirection.ASC);

			var usersDbo = await _dboManager.GetListAsync<UserDbo>(order: orderSettings);

			var roles = await GetRolesAsync();
			var userRolesDict = new Dictionary<Guid, List<TfRole>>();
			foreach (var userRoleRelation in await _dboManager.GetListAsync<UserRoleDbo>())
			{
				if (!userRolesDict.ContainsKey(userRoleRelation.UserId))
					userRolesDict[userRoleRelation.UserId] = new List<TfRole>();

				userRolesDict[userRoleRelation.UserId].Add(roles.Single(x => x.Id == userRoleRelation.RoleId));
			}

			List<TfUser> users = new List<TfUser>();
			foreach (var userDbo in usersDbo)
			{
				var userBuilder = new TfUserBuilder(this, userDbo.Id)
					.WithEmail(userDbo.Email)
					.WithFirstName(userDbo.FirstName)
					.WithLastName(userDbo.LastName)
					.CreatedOn(userDbo.CreatedOn)
					.Enabled(userDbo.Enabled)
					.WithPassword(userDbo.Password);

				if (userDbo.Settings != null)
				{
					userBuilder
						.WithThemeMode(userDbo.Settings.ThemeMode)
						.WithOpenSidebar(userDbo.Settings.IsSidebarOpen)
						.WithCultureCode(userDbo.Settings.CultureName)
						.WithStartUpUrl(userDbo.Settings.StartUpUrl)
						.WithPageSize(userDbo.Settings.PageSize)
						.WithViewPresetColumnPersonalizations(userDbo.Settings.ViewPresetColumnPersonalizations)
						.WithViewPresetSortPersonalizations(userDbo.Settings.ViewPresetSortPersonalizations);
				}

				if (userRolesDict.ContainsKey(userDbo.Id))
					userBuilder.WithRoles(userRolesDict[userDbo.Id].ToArray());

				users.Add(userBuilder.Build());
			}

			return users.AsReadOnly();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public async Task<TfUser> SaveUserAsync(
		TfUser user)
	{
		try
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (user.Id == Guid.Empty)
				return await CreateUserAsync(user);
			else
			{
				var existingUser = await GetUserAsync(user.Id);
				if (existingUser is not null)
					return await UpdateUserAsync(user);
				else
					return await CreateUserAsync(user);
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public async Task<TfUser> CreateUserAsync(
		TfUser user)
	{
		new UserValidator(this)
			.ValidateCreate(user)
			.ToValidationException()
			.ThrowIfContainsErrors();

		UserDbo userDbo = new UserDbo
		{
			Id = Guid.NewGuid(),
			CreatedOn = user.CreatedOn,
			Email = user.Email,
			Enabled = user.Enabled,
			FirstName = user.FirstName,
			LastName = user.LastName,
			Password = user.Password.ToMD5Hash(),
			Settings = user.Settings,
			XSearch = $"{user.FirstName} {user.LastName} {user.Email}"
		};

		using (TfDatabaseTransactionScope scope = _dbService.CreateTransactionScope())
		{
			bool success = await _dboManager.InsertAsync<UserDbo>(userDbo);
			if (!success)
				throw new TfDboServiceException("Insert<UserDbo> failed");

			foreach (var role in user.Roles)
			{
				var dbo = new UserRoleDbo { RoleId = role.Id, UserId = userDbo.Id };

				success = await _dboManager.InsertAsync<UserRoleDbo>(dbo);
				if (!success)
					throw new TfDboServiceException("Insert<UserRoleDbo> failed");
			}

			scope.Complete();

			await PublishEventWithScopeAsync(new TfUserCreatedEvent(await GetUserAsync(userDbo.Id)));
		}

		return await GetUserAsync(userDbo.Id);
	}

	public async Task<TfUser> CreateUserWithFormAsync(TfUserManageForm form)
	{
		var user = new TfUser
		{
			Id = form.Id,
			CreatedOn = DateTime.Now,
			Email = form.Email,
			FirstName = form.FirstName,
			LastName = form.LastName,
			Enabled = form.Enabled,
			Password = form.Password ?? String.Empty,
			Roles = new List<TfRole>().AsReadOnly(),
			Settings = new TfUserSettings
			{
				ThemeMode = form.ThemeMode,
				IsSidebarOpen = form.IsSidebarOpen,
				CultureName = form.Culture?.CultureInfo.Name ?? TfConstants.DefaultCulture.Name,
			}
		};
		user = await CreateUserAsync(user);
		return user;
	}

	public async Task<TfUser> UpdateUserAsync(
		TfUser user)
	{
		new UserValidator(this)
			.ValidateUpdate(user)
			.ToValidationException()
			.ThrowIfContainsErrors();

		//if password is changed, hash new password
		var existingUser = await GetUserAsync(user.Id);
		string password = user.Password;
		if (existingUser.Password != user.Password)
			password = user.Password.ToMD5Hash();

		UserDbo userDbo = new UserDbo
		{
			Id = user.Id,
			CreatedOn = user.CreatedOn,
			Email = user.Email,
			Enabled = user.Enabled,
			FirstName = user.FirstName,
			LastName = user.LastName,
			Password = password,
			Settings = user.Settings,
			XSearch = $"{user.FirstName} {user.LastName} {user.Email}"
		};

		using (TfDatabaseTransactionScope scope = _dbService.CreateTransactionScope())
		{
			//update user info
			bool success = await _dboManager.UpdateAsync<UserDbo>(userDbo);
			if (!success)
				throw new TfDboServiceException("UpdateAsync<UserDbo> failed");

			//remove old roles
			foreach (var role in existingUser.Roles)
			{
				var dbId = new Dictionary<string, Guid>
				{
					{ nameof(UserRoleDbo.UserId), user.Id }, { nameof(UserRoleDbo.RoleId), role.Id }
				};

				success = await _dboManager.DeleteAsync<UserRoleDbo>(dbId);

				if (!success)
					throw new TfDboServiceException("DeleteAsync<UserRoleDbo> failed");
			}

			//add new roles
			foreach (var role in user.Roles)
			{
				var dbo = new UserRoleDbo { RoleId = role.Id, UserId = userDbo.Id };

				success = await _dboManager.InsertAsync<UserRoleDbo>(dbo);

				if (!success)
					throw new TfDboServiceException("InsertAsync<UserRoleDbo> failed");
			}

			scope.Complete();

			await PublishEventWithScopeAsync(new TfUserUpdatedEvent(await GetUserAsync(userDbo.Id)));
		}

		return await GetUserAsync(userDbo.Id);
	}

	public async Task<TfUser> UpdateUserWithFormAsync(TfUserManageForm form)
	{
		var user = await GetUserAsync(form.Id);
		if (user is null)
			throw new Exception("user not found");
		TfUserBuilder userBuilder = CreateUserBuilder(user);
		userBuilder
			.WithEmail(form.Email)
			.WithFirstName(form.FirstName)
			.WithLastName(form.LastName)
			.Enabled(form.Enabled)
			.WithThemeMode(form.ThemeMode)
			.WithCultureCode(form.Culture?.CultureInfo.Name ?? TfConstants.DefaultCulture.Name)
			.WithRoles(user.Roles.ToArray());
		if (!String.IsNullOrWhiteSpace(form.Password))
			userBuilder.WithPassword(form.Password);

		user = userBuilder.Build();
		user = await SaveUserAsync(user);
		return user;
	}
	
	public async Task<TfUser> UpdateUserVisualPreferencesAsync(Guid userId, DesignThemeModes themeMode, TfCultureOption culture)
	{
		var user = await GetUserAsync(userId);
		if (user is null)
			throw new Exception("user not found");
		if(user.Settings.ThemeMode == themeMode) return user;
		
		TfUserBuilder userBuilder = CreateUserBuilder(user);
		userBuilder
			.WithThemeMode(themeMode)
			.WithCultureCode(culture.CultureName);
		user = userBuilder.Build();
		user = await SaveUserAsync(user);
		return user;
	}	

	public async Task<TfUser?> GetUserFromCookieAsync(IJSRuntime jsRuntime,
		AuthenticationStateProvider authStateProvider)
	{
		var user = (await authStateProvider.GetAuthenticationStateAsync())?.User;
		if (user is null) return null;
		//Temporary fix for multitab logout- we check the cookie as well
		var cookie = await new CookieService(jsRuntime).GetAsync(TfConstants.TEFTER_AUTH_COOKIE_NAME);
		if (cookie is null || user.Identity is null || !user.Identity.IsAuthenticated ||
		    (user.Identity as TfIdentity) is null ||
		    (user.Identity as TfIdentity)!.User is null)
		{
			return null;
		}

		var tfUser = ((TfIdentity)user.Identity).User;
		if (tfUser is null) return null;

		return tfUser;
	}

	public bool UserHasAccess(TfUser user, NavigationManager navigator, string? urlOverride = null)
	{
		if (user.IsAdmin) return true;

		var routeData = navigator.GetRouteState(urlOverride);
		if (routeData.HasNode(RouteDataNode.Space, 0) && routeData.SpaceId is not null)
		{
			if (routeData.HasNode(RouteDataNode.Manage, 2)) return false;

			var space = GetSpace(routeData.SpaceId.Value);
			if (!space.IsPrivate) return true;

			if (space.Roles
			    .Select(x => x.Id)
			    .Intersect(user.Roles.Select(x => x.Id))
			    .Any())
				return true;
			return false;
		}
		else
		{
			return true;
		}
	}

	public async Task<TfUser> AddUserToRoleAsync(
		Guid userId,
		Guid roleId)
	{
		try
		{
			var userSM = await GetUserAsync(userId);
			if (userSM is null)
				throw new Exception("User not found");

			var roleSM = await GetRoleAsync(roleId);
			if (roleSM is null)
				throw new Exception("Role not found");

			if (userSM.Roles.Any(x => x.Id == roleId))
				return userSM;

			await AddUsersRoleAsync(new List<TfUser> { userSM }, roleSM);
			return await GetUserAsync(userId);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public Task AddUsersRoleAsync(
		List<TfUser> users,
		TfRole role)
	{
		try
		{
			AddUsersRole(users, role);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}

		return Task.CompletedTask;
	}

	public async Task<TfUser> RemoveUserFromRoleAsync(
		Guid userId,
		Guid roleId)
	{
		try
		{
			var userSM = await GetUserAsync(userId);
			if (userSM is null)
				throw new Exception("User not found");

			var roleSM = await GetRoleAsync(roleId);
			if (roleSM is null)
				throw new Exception("Role not found");

			if (!userSM.Roles.Any(x => x.Id == roleId))
				return userSM;
			await RemoveUsersRoleAsync(new List<TfUser> { userSM }, roleSM);
			return await GetUserAsync(userId);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public Task RemoveUsersRoleAsync(
		List<TfUser> users,
		TfRole role)
	{
		try
		{
			RemoveUsersRole(users, role);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}

		return Task.CompletedTask;
	}

	public async Task<TfUser> SetStartUpUrl(Guid userId,
		string? url)
	{
		var user = GetUser(userId);
		var userBld = CreateUserBuilder(user);
		userBld
			.WithStartUpUrl(url);

		await SaveUserAsync(userBld.Build());
		var result = GetUser(userId);
		await PublishEventWithScopeAsync(new TfUserUpdatedEvent(result));
		return result;
	}

	public virtual async Task<TfUser> SetUserCulture(Guid userId, string cultureCode)
	{
		TfUser user = GetUser(userId);
		var userBld = CreateUserBuilder(user);
		userBld.WithCultureCode(cultureCode);
		await SaveUserAsync(userBld.Build());
		var result = GetUser(userId);
		await PublishEventWithScopeAsync(new TfUserUpdatedEvent(result));
		return result;
	}

	public virtual async Task<TfUser> SetPageSize(Guid userId, int? pageSize)
	{
		TfUser user = GetUser(userId);
		var userBld = CreateUserBuilder(user);
		userBld.WithPageSize(pageSize);
		await SaveUserAsync(userBld.Build());
		var result = GetUser(userId);
		await PublishEventWithScopeAsync(new TfUserUpdatedEvent(result));
		return result;
	}

	public virtual async Task<TfUser> SetViewPresetColumnPersonalization(Guid userId, Guid spaceViewId, Guid? presetId,
		Guid spaceViewColumnId, short? width)
	{
		if (width is null)
		{
			return await RemoveSpaceViewPersonalizations(userId, spaceViewId, presetId);
		}

		TfUser user = GetUser(userId);
		if (user is null) throw new Exception("User not found");
		var spaceView = GetSpaceView(spaceViewId);
		if (spaceView is null) throw new Exception("Space view not found");
		var allPersonalization = user.Settings.ViewPresetColumnPersonalizations.ToList();
		var personalizationIndex = allPersonalization.FindIndex(x =>
			x.SpaceViewId == spaceViewId
			&& x.SpaceViewColumnId == spaceViewColumnId
			&& x.PresetId == presetId);
		if (personalizationIndex == -1)
		{
			allPersonalization.Add(new TfViewPresetColumnPersonalization
			{
				SpaceViewColumnId = spaceViewColumnId, SpaceViewId = spaceViewId, PresetId = presetId, Width = width
			});
		}
		else
		{
			allPersonalization[personalizationIndex] = allPersonalization[personalizationIndex] with { Width = width };
		}

		var userBld = CreateUserBuilder(user);
		userBld.WithViewPresetColumnPersonalizations(allPersonalization);
		await SaveUserAsync(userBld.Build());
		var result = GetUser(userId);
		await PublishEventWithScopeAsync(new TfUserUpdatedEvent(result));
		return result;
	}

	public virtual List<TfSortQuery> CalculateViewPresetSortPersonalization(List<TfSortQuery> currentSorts,
		Guid spaceViewId, Guid spaceViewColumnId, bool hasShiftKey)
	{
		if (currentSorts is null) currentSorts = new();
		currentSorts = currentSorts.ToList();
		var spaceView = GetSpaceView(spaceViewId);
		if (spaceView is null) throw new Exception("Space view not found");
		var columns = GetSpaceViewColumnsList(spaceViewId);
		var column = columns.FirstOrDefault(x => x.Id == spaceViewColumnId);
		if (column is null) throw new Exception("Space view column not found");

		var columnIndex = currentSorts.FindIndex(x => x.Name == column.QueryName);
		if (!hasShiftKey)
		{
			//if column found toggle its state between asc, desc, null
			if (columnIndex > -1)
			{
				currentSorts = new List<TfSortQuery> { currentSorts[columnIndex] };
				if (currentSorts[0].Direction == (int)TfSortDirection.ASC)
				{
					currentSorts[0].Direction = (int)TfSortDirection.DESC;
				}
				else if (currentSorts[0].Direction == (int)TfSortDirection.DESC)
				{
					currentSorts = new();
				}
			}
			else
			{
				currentSorts = new List<TfSortQuery>
				{
					new TfSortQuery { Name = column.QueryName, Direction = (int)TfSortDirection.ASC }
				};
			}
		}
		else
		{
			if (columnIndex > -1)
			{
				if (currentSorts[columnIndex].Direction == (int)TfSortDirection.ASC)
				{
					currentSorts[columnIndex].Direction = (int)TfSortDirection.DESC;
				}
				else if (currentSorts[columnIndex].Direction == (int)TfSortDirection.DESC)
				{
					currentSorts.RemoveAt(columnIndex);
				}
			}
			else
			{
				var columnSort = new TfSortQuery { Name = column.QueryName, Direction = (int)TfSortDirection.ASC };
				currentSorts.Add(columnSort);
			}
		}

		return currentSorts;
	}

	public virtual async Task<TfUser> RemoveSpaceViewPersonalizations(Guid userId, Guid spaceViewId, Guid? presetId)
	{
		TfUser user = GetUser(userId);
		var otherPersonalization = user.Settings.ViewPresetColumnPersonalizations.Where(x =>
			!(x.SpaceViewId == spaceViewId && x.PresetId == presetId)).ToList();
		var otherSorts = user.Settings.ViewPresetSortPersonalizations.Where(x =>
			!(x.SpaceViewId == spaceViewId && x.PresetId == presetId)).ToList();
		var userBld = CreateUserBuilder(user);
		userBld.WithViewPresetColumnPersonalizations(otherPersonalization);
		userBld.WithViewPresetSortPersonalizations(otherSorts);
		await SaveUserAsync(userBld.Build());
		var result = GetUser(userId);
		await PublishEventWithScopeAsync(new TfUserUpdatedEvent(result));
		return result;
	}

	#region <--- validation --->

	internal class UserValidator : AbstractValidator<TfUser>
	{
		public UserValidator(
			ITfService tfService)
		{
			RuleSet("general", () =>
			{
				RuleFor(user => user.Email)
					.NotEmpty()
					.WithMessage("The email is required.");

				RuleFor(user => user.Email)
					.Must(email => { return email.IsEmail(); })
					.WithMessage("The email is incorrect.");

				RuleFor(user => user.FirstName)
					.NotEmpty()
					.WithMessage("The first name is required.");

				RuleFor(user => user.LastName)
					.NotEmpty()
					.WithMessage("The last name is required.");

				RuleFor(user => user.Password)
					.NotEmpty()
					.WithMessage("The password is required.");
			});

			RuleSet("create", () =>
			{
				RuleFor(user => user.Id)
					.Must(id => { return tfService.GetUser(id) == null; })
					.WithMessage("There is already existing user with specified id.");

				RuleFor(user => user.Email)
					.Must(email => { return tfService.GetUser(email) == null; })
					.WithMessage("There is already existing user with specified email.");
			});

			RuleSet("update", () =>
			{
				RuleFor(user => user.Id)
					.NotEmpty()
					.WithMessage("User identifier is not provided.");

				RuleFor(user => user.Id)
					.Must(id => { return tfService.GetUser(id) != null; })
					.WithMessage("There is no existing user for specified identifier.");

				RuleFor(user => user.Email)
					.Must((user, email) =>
					{
						var existingUser = tfService.GetUser(user.Email);
						return !(existingUser != null && existingUser.Id != user.Id);
					})
					.WithMessage("There is already existing user with specified email.");
			});
		}

		public ValidationResult ValidateCreate(
			TfUser user)
		{
			if (user == null)
				return new ValidationResult(new[] { new ValidationFailure("", "The user instance is null.") });

			return this.Validate(user, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(
			TfUser user)
		{
			if (user == null)
				return new ValidationResult(new[] { new ValidationFailure("", "The user instance is null.") });

			return this.Validate(user, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}
	}

	#endregion
}