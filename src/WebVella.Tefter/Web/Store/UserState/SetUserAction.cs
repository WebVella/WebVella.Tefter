namespace WebVella.Tefter.Web.Store.UserState;

/// <summary>
/// Called from the login component
/// </summary>
public record SetUserAction {

	public User User { get; }
	public SetUserAction(User user)
	{
		User = user;
	}
}
