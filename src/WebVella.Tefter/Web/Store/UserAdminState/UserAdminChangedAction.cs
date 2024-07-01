namespace WebVella.Tefter.Web.Store.UserAdminState;

public record UserAdminChangedAction
{
	public bool IsBusy { get; }
	public TucUser User { get; }

	public UserAdminChangedAction(
	bool isBusy,
	TucUser user)
	{
		IsBusy = isBusy;
		User = user;
	}
}
