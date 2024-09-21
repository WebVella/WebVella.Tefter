namespace WebVella.Tefter.Web.Store;


public partial record TfAppState
{
	public string HomeSearch { get; init; } = null;
	public bool HomeSearchInBookmarks { get; init; } = true;
	public bool HomeSearchInSaves { get; init; } = true;
	public bool HomeSearchInViews { get; init; } = true;
	public bool HomeSearchInSpaces { get; init; } = true;
	public List<TucTag> HomeTags { get; init; } = new();
	public List<TucBookmark> HomeBookmarks { get; init; } = new();
	public List<TucBookmark> HomeSaves { get; init; } = new();
	public List<TucSpaceView> HomeViews { get; init; } = new();
	public List<TucSearchResult> HomeSearchResults { get; init; } = new();
	
}
