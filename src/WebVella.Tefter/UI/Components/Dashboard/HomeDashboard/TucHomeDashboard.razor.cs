
namespace WebVella.Tefter.UI.Components;

public partial class TucHomeDashboard : TfBaseComponent
{
	private TfHomeDashboardData _data = new();
	private List<TfHowToItem> _howToItems = new();


	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_data = TfUIService.GetHomeDashboardData(TfAuthLayout.CurrentUser.Id);

		_howToItems.Add(new TfHowToItem
		{
			Title = "Create a data provider",
			Description = "The data provider defines the source, import process, storage structure, and synchronization rules for maintaining up-to-date data.",
			MoreInfoUrl = "https://tefter.webvella.com/en/docs/administrators/data-providers",
			FluentIconName = _data.ProvidersCount > 0 ? "Checkmark" : null,
			Count = _data.ProvidersCount > 0 ? null : 1,
			BackgroundColor = _data.ProvidersCount > 0 ? TfColor.Green100.GetColor().Value : null,
			Color = _data.ProvidersCount > 0 ? TfColor.Green700.GetColor().Value : null,
		});

		_howToItems.Add(new TfHowToItem
		{
			Title = "Create a space",
			Description = "The Space is a user interface area that organizes pages with a common purpose or those intended for use by a specific team.",
			MoreInfoUrl = "https://tefter.webvella.com/en/docs/end-users/space",
			FluentIconName = _data.SpacesCount > 0 ? "Checkmark" : null,
			Count = _data.SpacesCount > 0 ? null : 2,
			BackgroundColor = _data.SpacesCount > 0 ? TfColor.Green100.GetColor().Value : null,
			Color = _data.SpacesCount > 0 ? TfColor.Green700.GetColor().Value : null,
		});

		_howToItems.Add(new TfHowToItem
		{
			Title = "Create a 'Space view' page",
			Description = "The Space View presents data to users and provides core data collaboration features. It determines the display format for specific data columns, allows for quick filter setup.",
			MoreInfoUrl = "https://tefter.webvella.com/en/docs/end-users/space-page",
			FluentIconName = _data.SpacePagesCount > 0 ? "Checkmark" : null,
			Count = _data.SpacePagesCount > 0 ? null : 3,
			BackgroundColor = _data.SpacePagesCount > 0 ? TfColor.Green100.GetColor().Value : null,
			Color = _data.SpacePagesCount > 0 ? TfColor.Green700.GetColor().Value : null,
		});
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
		var navState = TfAuthLayout.NavigationState;
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
		if (!String.IsNullOrWhiteSpace(result.Url))
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