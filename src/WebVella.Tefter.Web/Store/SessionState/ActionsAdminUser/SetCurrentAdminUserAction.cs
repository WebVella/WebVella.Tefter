namespace WebVella.Tefter.Web.Store.SessionState;

public class SetCurrentAdminUser
{
	public User User { get; }

    public SetCurrentAdminUser(User user)
    {
		User = user;
    }
}
