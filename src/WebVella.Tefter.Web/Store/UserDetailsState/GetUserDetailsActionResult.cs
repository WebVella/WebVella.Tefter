namespace WebVella.Tefter.Web.Store.ThemeState;

public record GetUserDetailsActionResult
{
	public User User { get;}

	public GetUserDetailsActionResult(User user)
	{
		User = user;
	}
}
