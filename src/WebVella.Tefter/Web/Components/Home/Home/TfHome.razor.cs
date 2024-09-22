
namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Home.Home.TfHome", "WebVella.Tefter")]
public partial class TfHome : TfBaseComponent
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	private bool _isDataLoading = false;
	void AddInNotificationCenter()
	{
		MessageService.ShowMessageBar(options =>
		{
			options.Intent = MessageIntent.Error;
			options.Title = $"Simple message";
			options.Body = "<ul><li>test</li><li>test 2</li></ul>";
			options.Timestamp = DateTime.Now;
			options.Timeout = 5000;
			options.Section = TfConstants.MESSAGES_NOTIFICATION_CENTER;
		});
	}

	private void _onClick(TucSearchResult result)
	{
		Navigator.NavigateTo(result.Url);
	}

	private async Task _onSearch(string value)
	{
		var queryDict = new Dictionary<string, object>();
		queryDict[TfConstants.SearchQueryName] = value;
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	private async Task _onToggleFilter(string propName)
	{
		var queryDict = new Dictionary<string, object>();
		if (propName == "HomeSearchInBookmarks")
			queryDict[TfConstants.SearchInBookmarksQueryName] = TfAppState.Value.HomeSearchInBookmarks ? false : null;
		else if (propName == "HomeSearchInSaves")
			queryDict[TfConstants.SearchInSavesQueryName] = TfAppState.Value.HomeSearchInSaves ? false : null;
		else if (propName == "HomeSearchInViews")
			queryDict[TfConstants.SearchInViewsQueryName] = TfAppState.Value.HomeSearchInViews ? false : null;
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	private async Task _tagClick(TucTag tag)
	{
		var queryDict = new Dictionary<string, object>();
		queryDict[TfConstants.SearchQueryName] = "#" + tag.Label;
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
}