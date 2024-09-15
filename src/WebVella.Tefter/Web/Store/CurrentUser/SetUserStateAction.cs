namespace WebVella.Tefter.Web.Store;

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
