namespace WebVella.Tefter.Web.Store;

public record EmptyUserAdminAction
{
	public TucUser User { get; }

	public EmptyUserAdminAction()
	{
		User = null;
	}
}
