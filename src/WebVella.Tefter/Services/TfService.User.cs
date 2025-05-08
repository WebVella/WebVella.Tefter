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
	/// Saves a user to the system. Creates a new user if it does not exist, or updates an existing one.
	/// </summary>
	/// <param name="user">The user to save.</param>
	/// <returns>The saved <see cref="TfUser"/> instance.</returns>
	TfUser SaveUser(
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
	/// Removes a role from a list of users.
	/// </summary>
	/// <param name="users">The list of users to remove the role from.</param>
	/// <param name="role">The role to remove.</param>
	void RemoveUsersRole(
		List<TfUser> users,
		TfRole role);

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

	/// <summary>
	/// Asynchronously adds a role to a list of users.
	/// </summary>
	/// <param name="users">The list of users to add the role to.</param>
	/// <param name="role">The role to add.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	Task AddUsersRoleAsync(
		List<TfUser> users,
		TfRole role);
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
			var adminUsers = users.Where(x=> x.Roles.Any(r=> r.Id == TfConstants.ADMIN_ROLE_ID)).ToList();
			if(adminUsers.Count > 0)
				return adminUsers.First();

			if(users.Count > 0)
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
					.WithThemeColor(userDbo.Settings.ThemeColor)
					.WithOpenSidebar(userDbo.Settings.IsSidebarOpen)
					.WithCultureCode(userDbo.Settings.CultureName)
					.WithStartUpUrl(userDbo.Settings.StartUpUrl)
					.WithPageSize(userDbo.Settings.PageSize);
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
					.WithThemeColor(userDbo.Settings.ThemeColor)
					.WithOpenSidebar(userDbo.Settings.IsSidebarOpen)
					.WithCultureCode(userDbo.Settings.CultureName)
					.WithStartUpUrl(userDbo.Settings.StartUpUrl)
					.WithPageSize(userDbo.Settings.PageSize);
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
					.WithThemeColor(userDbo.Settings.ThemeColor)
					.WithOpenSidebar(userDbo.Settings.IsSidebarOpen)
					.WithCultureCode(userDbo.Settings.CultureName)
					.WithStartUpUrl(userDbo.Settings.StartUpUrl)
					.WithPageSize(userDbo.Settings.PageSize);
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
						.WithThemeColor(userDbo.Settings.ThemeColor)
						.WithOpenSidebar(userDbo.Settings.IsSidebarOpen)
						.WithCultureCode(userDbo.Settings.CultureName)
						.WithStartUpUrl(userDbo.Settings.StartUpUrl)
						.WithPageSize(userDbo.Settings.PageSize);
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

	private TfUser CreateUser(
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
			bool success = _dboManager.Insert<UserDbo>(userDbo);

			if (!success)
				throw new TfDboServiceException("Insert<UserDbo> failed");

			foreach (var role in user.Roles)
			{
				var dbo = new UserRoleDbo
				{
					RoleId = role.Id,
					UserId = userDbo.Id
				};

				success = _dboManager.Insert<UserRoleDbo>(dbo);
				if (!success)
					throw new TfDboServiceException("Insert<UserRoleDbo> failed");
			}

			scope.Complete();
		}

		return GetUser(userDbo.Id);
	}

	private TfUser UpdateUser(
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
				var dbId = new Dictionary<string, Guid> {
						{ nameof(UserRoleDbo.UserId), user.Id },
						{ nameof(UserRoleDbo.RoleId), role.Id }};

				success = _dboManager.Delete<UserRoleDbo>(dbId);

				if (!success)
					throw new TfDboServiceException("Delete<UserRoleDbo> failed");
			}

			//add new roles
			foreach (var role in user.Roles)
			{
				var dbo = new UserRoleDbo
				{
					RoleId = role.Id,
					UserId = userDbo.Id
				};

				success = _dboManager.Insert<UserRoleDbo>(dbo);

				if (!success)
					throw new TfDboServiceException("Insert<UserRoleDbo> failed");
			}

			scope.Complete();
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
						new UserRoleDbo
						{
							RoleId = role.Id,
							UserId = user.Id
						});

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

					var dbId = new Dictionary<string, Guid> {
						{ nameof(UserRoleDbo.UserId), user.Id },
						{ nameof(UserRoleDbo.RoleId), role.Id }};

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
			foreach (var userRoleRelation in await _dboManager.GetListAsync<UserRoleDbo>(id, nameof(UserRoleDbo.UserId)))
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
					.WithThemeColor(userDbo.Settings.ThemeColor)
					.WithOpenSidebar(userDbo.Settings.IsSidebarOpen)
					.WithCultureCode(userDbo.Settings.CultureName)
					.WithStartUpUrl(userDbo.Settings.StartUpUrl)
					.WithPageSize(userDbo.Settings.PageSize);
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
			foreach (var userRoleRelation in await _dboManager.GetListAsync<UserRoleDbo>(userDbo.Id, nameof(UserRoleDbo.UserId)))
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
					.WithThemeColor(userDbo.Settings.ThemeColor)
					.WithOpenSidebar(userDbo.Settings.IsSidebarOpen)
					.WithCultureCode(userDbo.Settings.CultureName)
					.WithStartUpUrl(userDbo.Settings.StartUpUrl)
					.WithPageSize(userDbo.Settings.PageSize);
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
			foreach (var userRoleRelation in await _dboManager.GetListAsync<UserRoleDbo>(userDbo.Id, nameof(UserRoleDbo.UserId)))
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
					.WithThemeColor(userDbo.Settings.ThemeColor)
					.WithOpenSidebar(userDbo.Settings.IsSidebarOpen)
					.WithCultureCode(userDbo.Settings.CultureName)
					.WithStartUpUrl(userDbo.Settings.StartUpUrl)
					.WithPageSize(userDbo.Settings.PageSize);
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
						.WithThemeColor(userDbo.Settings.ThemeColor)
						.WithOpenSidebar(userDbo.Settings.IsSidebarOpen)
						.WithCultureCode(userDbo.Settings.CultureName)
						.WithStartUpUrl(userDbo.Settings.StartUpUrl)
						.WithPageSize(userDbo.Settings.PageSize);
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

	private async Task<TfUser> CreateUserAsync(
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
				var dbo = new UserRoleDbo
				{
					RoleId = role.Id,
					UserId = userDbo.Id
				};

				success = await _dboManager.InsertAsync<UserRoleDbo>(dbo);
				if (!success)
					throw new TfDboServiceException("Insert<UserRoleDbo> failed");
			}

			scope.Complete();
		}

		return await GetUserAsync(userDbo.Id);
	}

	private async Task<TfUser> UpdateUserAsync(
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
				var dbId = new Dictionary<string, Guid> {
						{ nameof(UserRoleDbo.UserId), user.Id },
						{ nameof(UserRoleDbo.RoleId), role.Id }};

				success = await _dboManager.DeleteAsync<UserRoleDbo>(dbId);

				if (!success)
					throw new TfDboServiceException("DeleteAsync<UserRoleDbo> failed");
			}

			//add new roles
			foreach (var role in user.Roles)
			{
				var dbo = new UserRoleDbo
				{
					RoleId = role.Id,
					UserId = userDbo.Id
				};

				success = await _dboManager.InsertAsync<UserRoleDbo>(dbo);

				if (!success)
					throw new TfDboServiceException("InsertAsync<UserRoleDbo> failed");
			}

			scope.Complete();
		}

		return await GetUserAsync(userDbo.Id);
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