namespace WebVella.Tefter.Web.Store.UserAdminState;

public record EmptyUserAdminAction
{
	public bool IsBusy { get; }
	public TucUser User { get; }

	public EmptyUserAdminAction()
	{
		IsBusy = false;
		User = null;
	}
}
