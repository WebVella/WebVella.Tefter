namespace WebVella.Tefter.Demo.Data;

public static partial class SampleData
{
	public static User GetUser()
	{
		return User.GetFaker().Generate();
	}
}
