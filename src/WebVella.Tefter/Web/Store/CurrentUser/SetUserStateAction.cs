namespace WebVella.Tefter.Web.Store;

public record SetUserStateAction : TfBaseAction
{
	public TucUser User { get; }

	public List<TucSpace> UserSpaces { get; }

	internal SetUserStateAction(
		TfBaseComponent component,
		TucUser user,
		List<TucSpace> userSpaces)
	{
		Component = component;
		User = user;
		UserSpaces = userSpaces;
	}
}
