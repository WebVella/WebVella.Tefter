namespace WebVella.Tefter.Web.Store.DataProviderDetailsState;

public record SetDataProviderDetailsAction
{
	public User User { get; }

	public SetDataProviderDetailsAction(User user)
	{
		User = user;
	}
}
