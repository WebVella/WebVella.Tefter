namespace WebVella.Tefter.Web.Store.UserState;

public record SetUserAction {

	public TucUser User { get; }
	public SetUserAction(TucUser user)
	{
		User = user;
	}
}
