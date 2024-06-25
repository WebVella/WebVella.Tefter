namespace WebVella.Tefter.Web.Store.UserDetailsState;

public record SetUserDetailsAction
{
	public User User { get; }

	public SetUserDetailsAction(User user)
	{
		User = user;
	}
}
