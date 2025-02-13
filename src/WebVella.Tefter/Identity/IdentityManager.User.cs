using System.Data;

namespace WebVella.Tefter.Identity;

public partial interface IIdentityManager
{
	UserBuilder CreateUserBuilder(User user = null);
	User GetUser(Guid id);
	User GetUser(string email);
	User GetUser(string email, string password);
	ReadOnlyCollection<User> GetUsers();
	User SaveUser(User user);
	Task<User> GetUserAsync(Guid id);
	Task<User> GetUserAsync(string email);
	Task<User> GetUserAsync(string email, string password);
	Task<ReadOnlyCollection<User>> GetUsersAsync();
	Task<User> SaveUserAsync(User user);
}

public partial class IdentityManager : IIdentityManager
{
	public UserBuilder CreateUserBuilder(User user = null)
	{
		return new UserBuilder(this, user);
	}

	public User GetUser(Guid id)
	{
		var userDbo = _dboManager.Get<UserDbo>(id);
		if (userDbo == null)
			return null;

		var roles = GetRoles();

		var userRoles = new List<Role>();
		foreach (var userRoleRelation in _dboManager.GetList<UserRoleDbo>(id, nameof(UserRoleDbo.UserId)))
			userRoles.Add(roles.Single(x => x.Id == userRoleRelation.RoleId));

		var userBuilder = new UserBuilder(this, userDbo.Id)
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

	public User GetUser(string email)
	{
		var valEx = new TfValidationException();

		if (email == null)
			valEx.AddValidationError(nameof(User.Email), "The email is required.");

		valEx.ThrowIfContainsErrors();

		var userDbo = _dboManager.Get<UserDbo>(email, nameof(UserDbo.Email));
		if (userDbo == null)
			return null;

		var roles = GetRoles();
		var userRoles = new List<Role>();
		foreach (var userRoleRelation in _dboManager.GetList<UserRoleDbo>(userDbo.Id, nameof(UserRoleDbo.UserId)))
			userRoles.Add(roles.Single(x => x.Id == userRoleRelation.RoleId));

		var userBuilder = new UserBuilder(this, userDbo.Id)
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

	public User GetUser(string email, string password)
	{
		var valEx = new TfValidationException();

		if (email == null)
			valEx.AddValidationError(nameof(User.Email), "The email is required.");

		if (password == null)
			valEx.AddValidationError(nameof(User.Password), "The password is required.");

		valEx.ThrowIfContainsErrors();


		var userDbo = _dboManager.Get<UserDbo>(email, nameof(UserDbo.Email));
		if (userDbo == null)
			return null;

		if (userDbo.Password != password.ToMD5Hash())
			return null;

		var roles = GetRoles();
		var userRoles = new List<Role>();
		foreach (var userRoleRelation in _dboManager.GetList<UserRoleDbo>(userDbo.Id, nameof(UserRoleDbo.UserId)))
			userRoles.Add(roles.Single(x => x.Id == userRoleRelation.RoleId));

		var userBuilder = new UserBuilder(this, userDbo.Id)
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

	public ReadOnlyCollection<User> GetUsers()
	{
		var orderSettings = new TfOrderSettings(nameof(UserDbo.LastName), OrderDirection.ASC)
				.Add(nameof(UserDbo.FirstName), OrderDirection.ASC);

		var usersDbo = _dboManager.GetList<UserDbo>(order: orderSettings);

		var roles = GetRoles();
		var userRolesDict = new Dictionary<Guid, List<Role>>();
		foreach (var userRoleRelation in _dboManager.GetList<UserRoleDbo>())
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

	public User SaveUser(User user)
	{
		if (user == null)
			throw new ArgumentNullException(nameof(user));

		if (user.Id == Guid.Empty)
			return CreateUser(user);
		else
			return UpdateUser(user);
	}

	private User CreateUser(User user)
	{
		_userValidator
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

	private User UpdateUser(User user)
	{
		_userValidator
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

	public async Task<User> GetUserAsync(Guid id)
	{
		var userDbo = await _dboManager.GetAsync<UserDbo>(id);
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

	public async Task<User> GetUserAsync(string email)
	{
		var valEx = new TfValidationException();

		if (email == null)
			valEx.AddValidationError(nameof(User.Email), "The email is required.");

		valEx.ThrowIfContainsErrors();

		var userDbo = await _dboManager.GetAsync<UserDbo>(email, nameof(UserDbo.Email));
		if (userDbo == null)
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

	public async Task<User> GetUserAsync(string email, string password)
	{
		var valEx = new TfValidationException();

		if (email == null)
			valEx.AddValidationError(nameof(User.Email), "The email is required.");

		if (string.IsNullOrWhiteSpace(password))
			valEx.AddValidationError(nameof(User.Password), "The password is required.");

		valEx.ThrowIfContainsErrors();


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

	public async Task<ReadOnlyCollection<User>> GetUsersAsync()
	{
		var orderSettings = new TfOrderSettings(nameof(UserDbo.LastName), OrderDirection.ASC)
				.Add(nameof(UserDbo.FirstName), OrderDirection.ASC);

		var usersDbo = await _dboManager.GetListAsync<UserDbo>(order: orderSettings);

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

	public async Task<User> SaveUserAsync(User user)
	{
		if (user == null)
			throw new ArgumentNullException(nameof(user));

		if (user.Id == Guid.Empty)
			return await CreateUserAsync(user);
		else
			return await UpdateUserAsync(user);
	}

	private async Task<User> CreateUserAsync(User user)
	{
		_userValidator
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

	private async Task<User> UpdateUserAsync(User user)
	{
		_userValidator
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
}
