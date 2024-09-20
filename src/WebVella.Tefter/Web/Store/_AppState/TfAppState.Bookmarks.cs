namespace WebVella.Tefter.Web.Store;
public partial record TfAppState
{
	public List<TucBookmark> CurrentUserBookmarks { get; init; } = null;
	public List<TucBookmark> CurrentUserSaves { get; init; } = null;
	public Guid? ActiveSaveId { get; init; } = null;
}
