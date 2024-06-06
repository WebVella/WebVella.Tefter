namespace WebVella.Tefter.Demo.Data;

public static partial class SampleData
{
	private static User _user = null;
	public static User GetUser()
	{
		if (_user is not null) return _user;
		_user = User.GetFaker().Generate();
		return _user;
	}
}
