using System.Data;

namespace WebVella.Tefter.Identity;

public partial interface IIdentityManager
{
	UserBuilder CreateUserBuilder(User user = null);
	Result<User> GetUser(Guid id);
	Result<User> GetUser(string email);
	Result<User> GetUser(string email, string password);
	Result<ReadOnlyCollection<User>> GetUsers(string search = null, int? page = null, int? pageSize = null);
	Result<User> SaveUser(User user);
	Task<Result<User>> GetUserAsync(Guid id);
	Task<Result<User>> GetUserAsync(string email);
	Task<Result<User>> GetUserAsync(string email, string password);
	Task<Result<ReadOnlyCollection<User>>> GetUsersAsync(string search = null, int? page = null, int? pageSize = null);
	Task<Result<User>> SaveUserAsync(User user);
}

public partial class IdentityManager : IIdentityManager
{
	public UserBuilder CreateUserBuilder(User user = null)
	{
		return new UserBuilder(this, user);
	}

	public Result<User> GetUser(Guid id)
	{
		var userDbo = _dboManager.Get<UserDbo>(id);
		if (userDbo == null)
			return Result.Ok();

		var roles = GetRoles().Value;
		var userRoles = new List<Role>();
		foreach (var userRoleRelation in _dboManager.GetList<UserRoleDbo>(id, nameof(UserRoleDbo.UserId)))
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
				.WithThemeMode(userDbo.Settings.ThemeMode)
				.WithThemeColor(userDbo.Settings.ThemeColor)
				.WithOpenSidebar(userDbo.Settings.IsSidebarOpen)
				.WithCultureCode(userDbo.Settings.CultureCode);
		}

		return Result.Ok(userBuilder.Build());
	}

	public Result<User> GetUser(string email)
	{
		var userDbo = _dboManager.Get<UserDbo>(email, nameof(UserDbo.Email));
		if (userDbo == null)
			return Result.Ok();

		var roles = GetRoles().Value;
		var userRoles = new List<Role>();
		foreach (var userRoleRelation in _dboManager.GetList<UserRoleDbo>(userDbo.Id, nameof(UserRoleDbo.UserId)))
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
				.WithThemeMode(userDbo.Settings.ThemeMode)
				.WithThemeColor(userDbo.Settings.ThemeColor)
				.WithOpenSidebar(userDbo.Settings.IsSidebarOpen)
				.WithCultureCode(userDbo.Settings.CultureCode);
		}

		return Result.Ok(userBuilder.Build());
	}

	public Result<User> GetUser(string email, string password)
	{
		if (email == null)
			throw new ArgumentNullException("email");

		if (password == null)
			throw new ArgumentNullException("password");

		var userDbo = _dboManager.Get<UserDbo>(email, nameof(UserDbo.Email));
		if (userDbo == null)
			return Result.Ok();

		if (userDbo.Password != password.ToMD5Hash())
			return Result.Ok();

		var roles = GetRoles().Value;
		var userRoles = new List<Role>();
		foreach (var userRoleRelation in _dboManager.GetList<UserRoleDbo>(userDbo.Id, nameof(UserRoleDbo.UserId)))
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
				.WithThemeMode(userDbo.Settings.ThemeMode)
				.WithThemeColor(userDbo.Settings.ThemeColor)
				.WithOpenSidebar(userDbo.Settings.IsSidebarOpen)
				.WithCultureCode(userDbo.Settings.CultureCode);
		}

		return Result.Ok(userBuilder.Build());
	}

	public Result<ReadOnlyCollection<User>> GetUsers(string search = null, int? page = null, int? pageSize = null)
	{
		var orderSettings = new OrderSettings(nameof(UserDbo.LastName), OrderDirection.ASC)
				.Add(nameof(UserDbo.FirstName), OrderDirection.ASC);

		var usersDbo = _dboManager.GetList<UserDbo>(page, pageSize, orderSettings, search);

		var roles = GetRoles().Value;
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
				.WithPassword(string.Empty);

			if (userDbo.Settings != null)
			{
				userBuilder
					.WithThemeMode(userDbo.Settings.ThemeMode)
					.WithThemeColor(userDbo.Settings.ThemeColor)
					.WithOpenSidebar(userDbo.Settings.IsSidebarOpen)
					.WithCultureCode(userDbo.Settings.CultureCode);
			}

			if (userRolesDict.ContainsKey(userDbo.Id))
				userBuilder.WithRoles(userRolesDict[userDbo.Id].ToArray());

			users.Add(userBuilder.Build());
		}

		return Result.Ok(users.AsReadOnly());
	}

	public Result<User> SaveUser(User user)
	{
		if (user == null)
			throw new ArgumentNullException(nameof(user));

		if (user.Id == Guid.Empty)
			return CreateUser(user);
		else
			return UpdateUser(user);
	}

	private Result<User> CreateUser(User user)
	{
		ValidationResult result = _userValidator.ValidateCreate(user);

		if (!result.IsValid)
			return result.ToResult<User>();

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
			bool success = _dboManager.Insert<UserDbo>(userDbo);

			if (!success)
				return Result.Fail(new DboManagerError("InsertAsync", userDbo));

			foreach (var role in user.Roles)
			{
				var dbo = new UserRoleDbo
				{
					RoleId = role.Id,
					UserId = userDbo.Id
				};

				success = _dboManager.Insert<UserRoleDbo>(dbo);

				if (!success)
					return Result.Fail(new DboManagerError("InsertAsync", dbo));
			}

			scope.Complete();
		}

		return GetUser(userDbo.Id);
	}

	private Result<User> UpdateUser(User user)
	{
		ValidationResult result = _userValidator.ValidateUpdate(user);

		if (!result.IsValid)
			return result.ToResult<User>();

		//if password is changed, hash new password
		var existingUser = GetUser(user.Id).Value;
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

		using (DatabaseTransactionScope scope = _dbService.CreateTransactionScope())
		{
			//update user info
			bool success = _dboManager.Update<UserDbo>(userDbo);

			if (!success)
				return Result.Fail(new DboManagerError("UpdateAsync", userDbo));

			//remove old roles
			foreach (var role in existingUser.Roles)
			{
				var dbId = new Dictionary<string, Guid> {
						{ nameof(UserRoleDbo.UserId), user.Id },
						{ nameof(UserRoleDbo.RoleId), role.Id }};

				success = _dboManager.Delete<UserRoleDbo>(dbId);

				if (!success)
					return Result.Fail(new DboManagerError("InsertAsync", dbId));
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
					return Result.Fail(new DboManagerError("InsertAsync", dbo));
			}

			scope.Complete();
		}

		return GetUser(userDbo.Id);
	}

	public async Task<Result<User>> GetUserAsync(Guid id)
	{
		var userDbo = await _dboManager.GetAsync<UserDbo>(id);
		if (userDbo == null)
			return Result.Ok();

		var roles = (await GetRolesAsync()).Value;
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
				.WithThemeMode(userDbo.Settings.ThemeMode)
				.WithThemeColor(userDbo.Settings.ThemeColor)
				.WithOpenSidebar(userDbo.Settings.IsSidebarOpen)
				.WithCultureCode(userDbo.Settings.CultureCode);
		}

		return Result.Ok(userBuilder.Build());
	}

	public async Task<Result<User>> GetUserAsync(string email)
	{
		var userDbo = await _dboManager.GetAsync<UserDbo>(email, nameof(UserDbo.Email));
		if (userDbo == null)
			return Result.Ok();

		var roles = (await GetRolesAsync()).Value;
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
				.WithThemeMode(userDbo.Settings.ThemeMode)
				.WithThemeColor(userDbo.Settings.ThemeColor)
				.WithOpenSidebar(userDbo.Settings.IsSidebarOpen)
				.WithCultureCode(userDbo.Settings.CultureCode);
		}

		return Result.Ok(userBuilder.Build());
	}

	public async Task<Result<User>> GetUserAsync(string email, string password)
	{
		if (email == null)
			throw new ArgumentNullException("email");

		if (password == null)
			throw new ArgumentNullException("password");

		var userDbo = await _dboManager.GetAsync<UserDbo>(email, nameof(UserDbo.Email));
		if (userDbo == null)
			return Result.Ok();

		if (userDbo.Password != password.ToMD5Hash())
			return Result.Ok();

		var roles = (await GetRolesAsync()).Value;
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
				.WithThemeMode(userDbo.Settings.ThemeMode)
				.WithThemeColor(userDbo.Settings.ThemeColor)
				.WithOpenSidebar(userDbo.Settings.IsSidebarOpen)
				.WithCultureCode(userDbo.Settings.CultureCode);
		}

		return Result.Ok(userBuilder.Build());
	}

	public async Task<Result<ReadOnlyCollection<User>>> GetUsersAsync(string search = null, int? page = null, int? pageSize = null)
	{
		var orderSettings = new OrderSettings(nameof(UserDbo.LastName), OrderDirection.ASC)
				.Add(nameof(UserDbo.FirstName), OrderDirection.ASC);

		var usersDbo = await _dboManager.GetListAsync<UserDbo>(page, pageSize, orderSettings, search);

		var roles = (await GetRolesAsync()).Value;
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
					.WithThemeMode(userDbo.Settings.ThemeMode)
					.WithThemeColor(userDbo.Settings.ThemeColor)
					.WithOpenSidebar(userDbo.Settings.IsSidebarOpen)
					.WithCultureCode(userDbo.Settings.CultureCode);
			}

			if (userRolesDict.ContainsKey(userDbo.Id))
				userBuilder.WithRoles(userRolesDict[userDbo.Id].ToArray());

			users.Add(userBuilder.Build());
		}

		return Result.Ok(users.AsReadOnly());
	}

	public async Task<Result<User>> SaveUserAsync(User user)
	{
		if (user == null)
			throw new ArgumentNullException(nameof(user));

		if (user.Id == Guid.Empty)
			return await CreateUserAsync(user);
		else
			return await UpdateUserAsync(user);
	}

	private async Task<Result<User>> CreateUserAsync(User user)
	{
		ValidationResult result = _userValidator.ValidateCreate(user);

		if (!result.IsValid)
			return result.ToResult<User>();

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
				return Result.Fail(new DboManagerError("InsertAsync", userDbo));

			foreach (var role in user.Roles)
			{
				var dbo = new UserRoleDbo { 
					RoleId = role.Id, 
					UserId = userDbo.Id 
				};

				success = await _dboManager.InsertAsync<UserRoleDbo>(dbo);
				
				if (!success)
					return Result.Fail(new DboManagerError("InsertAsync", dbo));
			}

			scope.Complete();
		}

		return await GetUserAsync(userDbo.Id);
	}

	private async Task<Result<User>> UpdateUserAsync(User user)
	{
		ValidationResult result = _userValidator.ValidateUpdate(user);

		if (!result.IsValid)
			return result.ToResult<User>();

		//if password is changed, hash new password
		var existingUser = (await GetUserAsync(user.Id)).Value;
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

		using (DatabaseTransactionScope scope = _dbService.CreateTransactionScope())
		{
			//update user info
			bool success = await _dboManager.UpdateAsync<UserDbo>(userDbo);

			if (!success)
				return Result.Fail(new DboManagerError("UpdateAsync", userDbo));

			//remove old roles
			foreach (var role in existingUser.Roles)
			{
				var dbId = new Dictionary<string, Guid> {
						{ nameof(UserRoleDbo.UserId), user.Id },
						{ nameof(UserRoleDbo.RoleId), role.Id }};

				success = await _dboManager.DeleteAsync<UserRoleDbo>(dbId);

				if (!success)
					return Result.Fail(new DboManagerError("InsertAsync", dbId));
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
					return Result.Fail(new DboManagerError("InsertAsync", dbo));
			}

			scope.Complete();
		}

		return await GetUserAsync(userDbo.Id);
	}
}
