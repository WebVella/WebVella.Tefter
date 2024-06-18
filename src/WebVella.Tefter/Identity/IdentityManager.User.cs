namespace WebVella.Tefter.Identity;

public partial interface IIdentityManager
{
	UserBuilder CreateUserBuilder(User user = null);
	Task<User> GetUserAsync(Guid id);
	Task<User> GetUserAsync(string email, string password);
	Task<ReadOnlyCollection<User>> GetUsersAsync(string search = null, int? page = null, int? pageSize = null);
	Task<User> SaveUserAsync(User user);
}

public partial class IdentityManager : IIdentityManager
{
	public UserBuilder CreateUserBuilder(User user = null)
	{
		return new UserBuilder(this, user);
	}

	public async Task<User> GetUserAsync(Guid id)
	{
		var userDbo = await _dboManager.GetAsync<UserDbo>(id, nameof(UserDbo.Id));
		if (userDbo == null)
			return null;

		var roles = await GetRolesAsync();
		var userRoles = new List<Role>();
		foreach (var userRoleRelation in await _dboManager.GetListAsync<UserRoleDbo>(id, nameof(UserRoleDbo.UserId)))
			userRoles.Add(roles.Single(x => x.Id == userRoleRelation.RoleId));

		var userBuilder = new UserBuilder(this, userDbo.Id)
			.WithEmail(userDbo.Email)
			.WithFirstName(userDbo.FirstName)
			.WithLastName(userDbo.LastName)
			.CreatedOn(userDbo.CreatedOn)
			.Enabled(userDbo.Enabled)
			.WithPassword(string.Empty)
			.WithRoles(userRoles.ToArray());

		if (userDbo.Settings != null)
		{
			userBuilder
				.WithUiTheme(userDbo.Settings.UiTheme)
				.WithUiColor(userDbo.Settings.UiColor)
				.WithOpenSidebar(userDbo.Settings.IsSidebarOpen);
		}

		return userBuilder.Build();
	}

	public async Task<User> GetUserAsync(string email, string password)
	{
		if (email == null)
			throw new ArgumentNullException("email");

		if (password == null)
			throw new ArgumentNullException("password");

		var userDbo = await _dboManager.GetAsync<UserDbo>(email, nameof(UserDbo.Email));
		if (userDbo == null)
			return null;

		if (userDbo.Password != password.ToMD5Hash())
			return null;

		var roles = await GetRolesAsync();
		var userRoles = new List<Role>();
		foreach (var userRoleRelation in await _dboManager.GetListAsync<UserRoleDbo>(userDbo.Id, nameof(UserRoleDbo.UserId)))
			userRoles.Add(roles.Single(x => x.Id == userRoleRelation.RoleId));

		var userBuilder = new UserBuilder(this, userDbo.Id)
			.WithEmail(userDbo.Email)
			.WithFirstName(userDbo.FirstName)
			.WithLastName(userDbo.LastName)
			.CreatedOn(userDbo.CreatedOn)
			.Enabled(userDbo.Enabled)
			.WithPassword(string.Empty)
			.WithRoles(userRoles.ToArray());

		if (userDbo.Settings != null)
		{
			userBuilder
				.WithUiTheme(userDbo.Settings.UiTheme)
				.WithUiColor(userDbo.Settings.UiColor)
				.WithOpenSidebar(userDbo.Settings.IsSidebarOpen);
		}

		return userBuilder.Build();
	}

	public async Task<ReadOnlyCollection<User>> GetUsersAsync(string search = null, int? page = null, int? pageSize = null)
	{
		var orderSettings = new OrderSettings(nameof(UserDbo.LastName), OrderDirection.ASC)
				.Add(nameof(UserDbo.FirstName), OrderDirection.ASC);

		var usersDbo = await _dboManager.GetListAsync<UserDbo>(page, pageSize, orderSettings, search);

		var roles = await GetRolesAsync();
		var userRolesDict = new Dictionary<Guid, List<Role>>();
		foreach (var userRoleRelation in await _dboManager.GetListAsync<UserRoleDbo>())
		{
			if (!userRolesDict.ContainsKey(userRoleRelation.UserId))
				userRolesDict[userRoleRelation.UserId] = new List<Role>();

			userRolesDict[userRoleRelation.UserId].Add(roles.Single(x => x.Id == userRoleRelation.RoleId));
		}

		List<User> users = new List<User>();
		foreach (var userDbo in usersDbo)
		{
			var userBuilder = new UserBuilder(this, userDbo.Id)
				.WithEmail(userDbo.Email)
				.WithFirstName(userDbo.FirstName)
				.WithLastName(userDbo.LastName)
				.CreatedOn(userDbo.CreatedOn)
				.Enabled(userDbo.Enabled)
				.WithPassword(string.Empty);

			if (userDbo.Settings != null)
			{
				userBuilder
					.WithUiTheme(userDbo.Settings.UiTheme)
					.WithUiColor(userDbo.Settings.UiColor)
					.WithOpenSidebar(userDbo.Settings.IsSidebarOpen);
			}

			if (userRolesDict.ContainsKey(userDbo.Id))
				userBuilder.WithRoles(userRolesDict[userDbo.Id].ToArray());

			users.Add(userBuilder.Build());
		}

		return users.AsReadOnly();
	}

	public async Task<User> SaveUserAsync(User user)
	{
		if (user == null)
			throw new ArgumentNullException(nameof(user));

		if (user.Id == Guid.Empty)
			return await CreateUser(user);
		else
			return await UpdateUser(user);
	}

	private async Task<User> CreateUser(User user)
	{
		//ValidateUserOnCreate(user);

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

		using (DatabaseTransactionScope scope = _dbService.CreateTransactionScope())
		{
			bool success = await _dboManager.InsertAsync<UserDbo>(userDbo);

			if (!success)
				throw new DatabaseException("InsertAsync<UserDbo> operation failed.");

			foreach (var role in user.Roles)
			{
				success = await _dboManager.InsertAsync<UserRoleDbo>(new UserRoleDbo { RoleId = role.Id, UserId = userDbo.Id });
				if (!success)
					throw new DatabaseException("InsertAsync<UserRole> operation failed.");
			}

			scope.Complete();
		}

		return await GetUserAsync(userDbo.Id);
	}

	private async Task<User> UpdateUser(User user)
	{
		return user;
	}
}
