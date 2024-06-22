namespace WebVella.Tefter.Web.Store.SessionState;
public partial record SessionState
{
	public User CurrentAdminUser { get; init; }
}
