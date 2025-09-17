namespace WebVella.Tefter.Models;
public class TfUserBuilder
{
	private readonly ITfService _tfService;
	private Guid _id;
	private string _email;
	private string _firstName;
	private string _lastName;
	private string _password;
	private bool _enabled;
	private DateTime _createdOn;

	private DesignThemeModes _themeMode;
	private TfColor? _themeColor;
	private bool _sidebarOpen;
	private string _cultureCode;
	private string? _startUpUrl;
	private int? _pageSize;
	private List<TfViewPresetColumnPersonalization> _columnPersonalization = new();
	private List<TfViewPresetSortPersonalization> _sortPersonalization = new();

	private List<TfRole> _roles = new();

	internal TfUserBuilder(
		ITfService tfService,
		TfUser user = null)
	{
		_tfService = tfService;
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
			_startUpUrl = (user?.Settings?.StartUpUrl) ?? string.Empty;
			_pageSize = user?.Settings?.PageSize;
			_roles = user.Roles.ToList();
		}
	}

	internal TfUserBuilder(
		ITfService tfService,
		Guid userId)
	{
		_tfService = tfService;
		_id = userId;
	}

	public TfUserBuilder WithId(
		Guid id)
	{
		_id = id;
		return this;
	}

	public TfUserBuilder WithEmail(
		string email)
	{
		_email = email;
		return this;
	}

	public TfUserBuilder WithPassword(
		string password)
	{
		_password = password;
		return this;
	}

	public TfUserBuilder WithFirstName(
		string firstName)
	{
		_firstName = firstName;
		return this;
	}

	public TfUserBuilder WithLastName(
		string lastName)
	{
		_lastName = lastName;
		return this;
	}

	public TfUserBuilder CreatedOn(
		DateTime createdOn)
	{
		_createdOn = createdOn;
		return this;
	}

	public TfUserBuilder Enabled(
		bool enabled)
	{
		_enabled = enabled;
		return this;
	}

	public TfUserBuilder WithThemeMode(
		DesignThemeModes themeMode)
	{
		_themeMode = themeMode;
		return this;
	}

	public TfUserBuilder WithThemeColor(
		TfColor? tfColor)
	{
		_themeColor = tfColor;
		return this;
	}

	public TfUserBuilder WithOpenSidebar(
		bool isOpen)
	{
		_sidebarOpen = isOpen;
		return this;
	}

	public TfUserBuilder WithCultureCode(
		string cultureCode)
	{
		_cultureCode = cultureCode;
		return this;
	}

	public TfUserBuilder WithStartUpUrl(
		string? url)
	{
		_startUpUrl = url;
		return this;
	}

	public TfUserBuilder WithPageSize(
		int? pageSize)
	{
		_pageSize = pageSize;
		return this;
	}

	public TfUserBuilder WithViewPresetColumnPersonalizations(
		List<TfViewPresetColumnPersonalization> columnPersonalizations)
	{
		if(columnPersonalizations is null)
			columnPersonalizations = new();
		_columnPersonalization = columnPersonalizations;
		return this;
	}

	public TfUserBuilder WithViewPresetSortPersonalizations(
		List<TfViewPresetSortPersonalization> sortPersonalizations)
	{
		if(sortPersonalizations is null)
			sortPersonalizations = new();
		_sortPersonalization = sortPersonalizations;
		return this;
	}

	public TfUserBuilder WithRoles(
		params TfRole[] roles)
	{
		if (roles == null || roles.Length == 0)
		{
			_roles.Clear();
			return this;
		}


		foreach (TfRole role in roles)
		{
			if (!_roles.Any(x => x.Id == role.Id))
				_roles.Add(role);
		}

		HashSet<Guid> newRolesHs = roles.Select(x => x.Id).Distinct().ToHashSet();
		var existingRoles = _roles.ToList();
		foreach (var role in existingRoles)
		{
			if (!newRolesHs.Contains(role.Id))
				_roles.Remove(role);
		}

		return this;
	}

	public TfUserBuilder RemoveRoles(
		params TfRole[] roles)
	{
		if (roles == null || roles.Length == 0)
			return this;

		foreach (TfRole role in roles)
		{
			if (_roles.Any(x => x.Id == role.Id))
			{
				var existingRole = _roles.FirstOrDefault(x => x.Id == role.Id);
				_roles.Remove(existingRole);
			}
		}

		return this;
	}

	public TfUser Build()
	{
		return new TfUser
		{
			Id = _id,
			CreatedOn = _createdOn,
			Email = _email,
			Enabled = _enabled,
			FirstName = _firstName,
			LastName = _lastName,
			Password = _password,
			Roles = _roles.AsReadOnly(),
			Settings = new TfUserSettings
			{
				IsSidebarOpen = _sidebarOpen,
				ThemeColor = _themeColor,
				ThemeMode = _themeMode,
				CultureName = _cultureCode,
				StartUpUrl = _startUpUrl,
				PageSize = _pageSize,
				ViewPresetColumnPersonalizations = _columnPersonalization,
				ViewPresetSortPersonalizations = _sortPersonalization,
			}
		};
	}

}
