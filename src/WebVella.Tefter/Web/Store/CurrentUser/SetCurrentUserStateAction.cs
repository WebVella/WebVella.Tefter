namespace WebVella.Tefter.Web.Store;

public record SetCurrentUserStateAction : TfBaseAction
{
	public TucUser User { get; }

	public List<TucSpace> UserSpaces { get; }

	internal SetCurrentUserStateAction(
		TfBaseComponent component,
		TucUser user,
		List<TucSpace> userSpaces)
	{
		Component = component;
		User = user;
		UserSpaces = userSpaces;
	}
}
