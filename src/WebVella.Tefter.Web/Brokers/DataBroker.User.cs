//namespace WebVella.Tefter.Web.Brokers;

//public partial interface IDataBroker
//{
//	ValueTask<List<UserDemo>> GetUsersAsync();
//	ValueTask<UserDemo> GetUserByIdAsync(Guid id);
//	ValueTask<UserDemo> GetUserByEmailAndPasswordAsync(string email, string password);
//	ValueTask<UserDemo> SetUserUISettingsAsync(Guid userId, DesignThemeModes themeMode, 
//		OfficeColor themeColor, bool sidebarExpanded, string cultureCode);
//}

//public partial class DataBroker : IDataBroker
//{
//	private static UserDemo _user;
//	private static List<UserDemo> _users;

//	public async ValueTask<List<UserDemo>> GetUsersAsync(){ 
//		if(_users is not null) return _users;
//		_users = new List<UserDemo>();	
//		for (int i = 0; i < 58; i++)
//		{
//			_users.Add(UserDemo.GetFaker().Generate());
//		}
//		return _users;
//	}

//	public async ValueTask<UserDemo> GetUserByIdAsync(Guid id){ 
//		if(_user is not null) return _user;
//		if(_users is null) await GetUsersAsync();
//		_user = _users[0];
//		return _user;
//	}

//	public async ValueTask<UserDemo> GetUserByEmailAndPasswordAsync(string email, string password)
//	{
//		if (_user is not null) return _user;
//		_user = UserDemo.GetFaker().Generate();
//		return _user;
//	}

//	public async ValueTask<UserDemo> SetUserUISettingsAsync(Guid userId, DesignThemeModes themeMode, 
//	OfficeColor themeColor, bool sidebarExpanded, string cultureCode)
//	{
//		var user =  await GetUserByIdAsync(userId);
//		_user = user with { 
	
//			ThemeMode = themeMode,
//			ThemeColor = themeColor,
//			SidebarExpanded = sidebarExpanded
//		};

//		return _user;
//	}

//}
