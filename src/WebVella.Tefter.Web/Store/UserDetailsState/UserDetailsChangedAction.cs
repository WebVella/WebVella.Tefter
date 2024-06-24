namespace WebVella.Tefter.Web.Store.UserDetailsState;

public record UserDetailsChangedAction
{
	public User User { get;}

	public UserDetailsChangedAction(User user)
	{
		User = user;
	}
}
