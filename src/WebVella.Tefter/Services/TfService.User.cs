namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	TfUserBuilder CreateUserBuilder(TfUser user = null);
	TfUser GetUser(Guid id);
	TfUser GetUser(string email);
	TfUser GetUser(string email, string password);
	ReadOnlyCollection<TfUser> GetUsers();
	TfUser SaveUser(TfUser user);
	Task<TfUser> GetUserAsync(Guid id);
	Task<TfUser> GetUserAsync(string email);
	Task<TfUser> GetUserAsync(string email, string password);
	Task<ReadOnlyCollection<TfUser>> GetUsersAsync();
	Task<TfUser> SaveUserAsync(TfUser user);
}

public partial class TfService : ITfService
{
	public TfUserBuilder CreateUserBuilder(TfUser user = null)
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

	public TfUser GetUser(Guid id)
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

	public TfUser GetUser(string email)
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

	public TfUser GetUser(string email, string password)
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

	public TfUser SaveUser(TfUser user)
	{
		try
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (user.Id == Guid.Empty)
				return CreateUser(user);
			else
				return UpdateUser(user);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	private TfUser CreateUser(TfUser user)
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

	private TfUser UpdateUser(TfUser user)
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

	public async Task<TfUser> GetUserAsync(Guid id)
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

	public async Task<TfUser> GetUserAsync(string email)
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

	public async Task<TfUser> GetUserAsync(string email, string password)
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

	public async Task<TfUser> SaveUserAsync(TfUser user)
	{
		try
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (user.Id == Guid.Empty)
				return await CreateUserAsync(user);
			else
				return await UpdateUserAsync(user);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	private async Task<TfUser> CreateUserAsync(TfUser user)
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

	private async Task<TfUser> UpdateUserAsync(TfUser user)
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

		public ValidationResult ValidateCreate(TfUser user)
		{
			if (user == null)
				return new ValidationResult(new[] { new ValidationFailure("", "The user instance is null.") });

			return this.Validate(user, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(TfUser user)
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