
namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.UI.Components.Dashboard.HomeDashboard.TucHomeDashboard", "WebVella.Tefter")]
public partial class TucHomeDashboard : TfBaseComponent
{
	[Inject] protected ITfUserUIService TfUserUIService { get; set; } = default!;
	[Inject] protected ITfDashboardUIService TfDashboardUIService { get; set; } = default!;
	[Inject] protected ITfNavigationUIService ITfNavigationUIService { get; set; } = default!;

	private TfHomeDashboardData _data = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		var user = await TfUserUIService.GetCurrentUserAsync();
		if (user is not null)
			_data = TfDashboardUIService.GetHomeDashboardData(user.Id);
	}

	private async Task _onSearch(string value)
	{
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.SearchQueryName, value}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	private async Task _onToggleFilter(string propName)
	{
		var navState = await ITfNavigationUIService.GetNavigationStateAsync(Navigator);
		var queryDict = new Dictionary<string, object>();
		if (propName == "HomeSearchInBookmarks")
			queryDict[TfConstants.SearchInBookmarksQueryName] = navState!.SearchInBookmarks ? false : null;
		else if (propName == "HomeSearchInSaves")
			queryDict[TfConstants.SearchInSavesQueryName] = navState!.SearchInSaves ? false : null;
		else if (propName == "HomeSearchInViews")
			queryDict[TfConstants.SearchInViewsQueryName] = navState!.SearchInViews ? false : null;
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	private void _onClick(TfSearchResult result)
	{
		if(!String.IsNullOrWhiteSpace(result.Url))
			Navigator.NavigateTo(result.Url);
	}

	private async Task _tagClick(TfTag tag)
	{
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.SearchQueryName, "#" + tag.Label}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
}