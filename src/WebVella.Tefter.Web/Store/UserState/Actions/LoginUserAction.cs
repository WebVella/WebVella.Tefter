namespace WebVella.Tefter.Web.Store.UserState;

public record LoginUserAction {
	public string Email { get; }
	public string Password { get; }

	public LoginUserAction(string email, string password)
	{
		Email = email?.Trim();
		Password = password?.Trim();
	}
}
