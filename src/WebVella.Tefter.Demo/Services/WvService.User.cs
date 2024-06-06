namespace WebVella.Tefter.Demo.Services;

public partial interface IWvService
{
	User GetUserByCookieValue(string encryptedCookie);
}

public partial class WvService : IWvService
{
	public User GetUserByCookieValue(string encryptedCookie)
	{ 
		return SampleData.GetUser();	
	}
}
