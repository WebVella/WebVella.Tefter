namespace WebVella.Tefter.Web.Brokers;

public partial interface IDataBroker
{
	ValueTask<List<User>> GetUsersAsync();
	ValueTask<User> GetUserByIdAsync(Guid id);
	ValueTask<User> GetUserByEmailAndPasswordAsync(string email, string password);
	ValueTask<User> SetUserUISettingsAsync(Guid userId, DesignThemeModes themeMode, OfficeColor themeColor, bool sidebarExpanded);
}

public partial class DataBroker : IDataBroker
{
	private static User _user;
	private static List<User> _users;

	public async ValueTask<List<User>> GetUsersAsync(){ 
		if(_users is not null) return _users;
		_users = new List<User>();	
		for (int i = 0; i < 58; i++)
		{
			_users.Add(User.GetFaker().Generate());
		}
		return _users;
	}

	public async ValueTask<User> GetUserByIdAsync(Guid id){ 
		if(_user is not null) return _user;
		if(_users is null) await GetUsersAsync();
		_user = _users[0];
		return _user;
	}

	public async ValueTask<User> GetUserByEmailAndPasswordAsync(string email, string password)
	{
		if (_user is not null) return _user;
		_user = User.GetFaker().Generate();
		return _user;
	}

	public async ValueTask<User> SetUserUISettingsAsync(Guid userId, DesignThemeModes themeMode, OfficeColor themeColor, bool sidebarExpanded)
	{
		var user =  await GetUserByIdAsync(userId);
		_user = user with { 
			ThemeMode = themeMode,
			ThemeColor = themeColor,
			SidebarExpanded = sidebarExpanded
		};

		return _user;
	}

}
