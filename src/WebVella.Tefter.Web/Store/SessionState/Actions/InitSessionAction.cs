namespace WebVella.Tefter.Web.Store.SessionState;

public record InitSessionAction
{
	public UserSession UserSession { get; }

	public InitSessionAction(UserSession userSession)
	{
		UserSession = userSession;
	}
}
