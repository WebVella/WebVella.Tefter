namespace WebVella.Tefter.Web.Store;

public partial record TfState
{
	public TucUser CurrentUser { get; init; }
	public List<TucSpace> CurrentUserSpaces { get; init; } = new();

}
