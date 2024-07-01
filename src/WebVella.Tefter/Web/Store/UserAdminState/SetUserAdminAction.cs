namespace WebVella.Tefter.Web.Store.UserAdminState;

public record SetUserAdminAction
{
	public bool IsBusy { get; }
	public TucUser User { get; }

	public SetUserAdminAction(
		bool isBusy,
		TucUser user)
	{
		IsBusy = isBusy;
		User = user;
	}
}
