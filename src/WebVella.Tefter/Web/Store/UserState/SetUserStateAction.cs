namespace WebVella.Tefter.Web.Store.UserState;

public record SetUserStateAction
{
	public TucUser User { get; }

	public List<TucSpace> UserSpaces { get; }

	internal SetUserStateAction(
		TucUser user,
		List<TucSpace> userSpaces)
	{
		User = user;
		UserSpaces = userSpaces;
	}
}
