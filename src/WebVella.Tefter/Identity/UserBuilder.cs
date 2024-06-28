namespace WebVella.Tefter.Identity;

public class UserBuilder
{
	private readonly IIdentityManager _identityManager;
	private Guid _id;
	private string _email;
	private string _firstName;
	private string _lastName;
	private string _password;
	private bool _enabled;
	private DateTime _createdOn;

	private DesignThemeModes _themeMode;
	private OfficeColor _themeColor;
	private bool _sidebarOpen;
	private string _cultureCode;

	private List<Role> _roles = new();

	internal UserBuilder(IIdentityManager identityManager, User user = null)
	{
		_identityManager = identityManager;
		if (user == null)
		{
			_id = Guid.Empty;
		}
		else
		{
			_id = user.Id;
			_email = user.Email;
			_firstName = user.FirstName;
			_lastName = user.LastName;
			_password = user.Password;
			_enabled = user.Enabled;
			_createdOn = user.CreatedOn;
			_themeMode = user.Settings.ThemeMode;
			_themeColor = user.Settings.ThemeColor;
			_sidebarOpen = (user?.Settings?.IsSidebarOpen) ?? true;
			_cultureCode = (user?.Settings?.CultureName) ?? string.Empty;
			_roles = user.Roles.ToList();
		}
	}

	internal UserBuilder(IIdentityManager identityManager, Guid userId)
	{
		_identityManager = identityManager;
		_id = userId;
	}

	public UserBuilder WithEmail(string email)
	{
		_email = email;
		return this;
	}

	public UserBuilder WithPassword(string password)
	{
		_password = password;
		return this;
	}

	public UserBuilder WithFirstName(string firstName)
	{
		_firstName = firstName;
		return this;
	}

	public UserBuilder WithLastName(string lastName)
	{
		_lastName = lastName;
		return this;
	}

	public UserBuilder CreatedOn(DateTime createdOn)
	{
		_createdOn = createdOn;
		return this;
	}

	public UserBuilder Enabled(bool enabled)
	{
		_enabled = enabled;
		return this;
	}

	public UserBuilder WithThemeMode(DesignThemeModes themeMode)
	{
		_themeMode = themeMode;
		return this;
	}

	public UserBuilder WithThemeColor(OfficeColor officeColor)
	{
		_themeColor = officeColor;
		return this;
	}

	public UserBuilder WithOpenSidebar(bool isOpen)
	{
		_sidebarOpen = isOpen;
		return this;
	}

	public UserBuilder WithCultureCode(string cultureCode)
	{
		_cultureCode = cultureCode;
		return this;
	}

	public UserBuilder WithRoles(params Role[] roles)
	{
		if (roles == null || roles.Length == 0)
		{
			_roles.Clear();
			return this;
		}

		
		foreach (Role role in roles)
		{
			if (!_roles.Any(x => x.Id == role.Id))
				_roles.Add(role);
		}

		HashSet<Guid> newRolesHs = roles.Select(x=>x.Id).Distinct().ToHashSet();
		var existingRoles = _roles.ToList();
		foreach (var role in existingRoles)
		{
			if (!newRolesHs.Contains(role.Id))
				_roles.Remove(role);
		}

		return this;
	}

	public UserBuilder RemoveRoles(params Role[] roles)
	{
		if (roles == null || roles.Length == 0)
			return this;

		foreach (Role role in roles)
		{
			if (_roles.Any(x => x.Id == role.Id))
			{
				var existingRole = _roles.FirstOrDefault(x => x.Id == role.Id);
				_roles.Remove(existingRole);
			}
		}

		return this;
	}

	public User Build()
	{
		return new User
		{
			Id = _id,
			CreatedOn = _createdOn,
			Email = _email,
			Enabled = _enabled,
			FirstName = _firstName,
			LastName = _lastName,
			Password = _password,
			Roles = _roles.AsReadOnly(),
			Settings = new UserSettings
			{
				IsSidebarOpen = _sidebarOpen,
				ThemeColor = _themeColor,
				ThemeMode = _themeMode,
				CultureName = _cultureCode
			}
		};
	}

}
