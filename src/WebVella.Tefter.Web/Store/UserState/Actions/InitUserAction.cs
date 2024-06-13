namespace WebVella.Tefter.Web.Store.UserState;

public record InitUserAction{
	public User User { get;}

	public InitUserAction(User user)
	{
		User = user;
	}
}
