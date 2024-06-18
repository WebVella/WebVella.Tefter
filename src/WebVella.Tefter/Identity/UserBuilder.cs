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
	private string _uiTheme;
	private string _uiColor;
	private bool _sidebarOpen;
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
			_password = string.Empty;
			_enabled = user.Enabled;
			_createdOn = user.CreatedOn;
			_uiTheme = user?.Settings?.UiTheme;
			_uiColor = user?.Settings?.UiColor;
			_sidebarOpen = (user?.Settings?.IsSidebarOpen) ?? true;
		}
	}

	internal UserBuilder(IIdentityManager identityManager, Guid userId )
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

	public UserBuilder WithUiTheme(string theme)
	{
		_uiTheme = theme;
		return this;
	}

	public UserBuilder WithUiColor(string color)
	{
		_uiColor = color;
		return this;
	}

	public UserBuilder WithOpenSidebar(bool isOpen)
	{
		_sidebarOpen = isOpen;
		return this;
	}

	public UserBuilder WithRoles(params Role[] roles)
	{
		if (roles == null || roles.Length == 0)
			return this;

		foreach (Role role in roles)
		{
			if (!_roles.Any(x => x.Id == role.Id))
				_roles.Add(role);
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
				UiColor = _uiColor,
				UiTheme = _uiTheme
			}
		};
	}

}
