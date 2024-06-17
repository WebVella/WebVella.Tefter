namespace WebVella.Tefter.Web.Brokers;

public partial interface IDataBroker
{
	ValueTask<User> GetUserByIdAsync(Guid id);
	ValueTask<User> GetUserByEmailAndPasswordAsync(string email, string password);
	ValueTask<User> SetUserUISettingsAsync(Guid userId, DesignThemeModes themeMode, OfficeColor themeColor, bool sidebarExpanded);
}

public partial class DataBroker : IDataBroker
{
	private static User _user;

	public async ValueTask<User> GetUserByIdAsync(Guid id){ 
		if(_user is not null) return _user;
		_user = User.GetFaker().Generate();
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
