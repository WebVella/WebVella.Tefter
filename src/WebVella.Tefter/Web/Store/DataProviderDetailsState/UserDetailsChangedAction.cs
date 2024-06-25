namespace WebVella.Tefter.Web.Store.DataProviderDetailsState;

public record DataProviderDetailsChangedAction
{
	public User User { get;}

	public DataProviderDetailsChangedAction(User user)
	{
		User = user;
	}
}
