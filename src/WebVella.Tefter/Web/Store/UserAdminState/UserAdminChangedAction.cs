namespace WebVella.Tefter.Web.Store.UserAdminState;

public record UserAdminChangedAction
{
	public TucUser User { get;}

	public UserAdminChangedAction(TucUser user)
	{
		User = user;
	}
}
