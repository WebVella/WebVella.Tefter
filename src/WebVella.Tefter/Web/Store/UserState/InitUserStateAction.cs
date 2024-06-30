namespace WebVella.Tefter.Web.Store.UserState;

public record InitUserStateAction
{
	public TucUser User { get; }
	internal InitUserStateAction(
		TucUser user)
	{
		User = user;
	}
}
