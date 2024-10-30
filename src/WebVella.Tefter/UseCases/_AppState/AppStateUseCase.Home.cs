namespace WebVella.Tefter.UseCases.AppState;
internal partial class AppStateUseCase
{
	internal async Task<(TfAppState, TfAuxDataState)> InitHomeAsync(IServiceProvider serviceProvider,
		TucUser currentUser, TfRouteState routeState,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
	{
		if (!(routeState.FirstNode == RouteDataFirstNode.Home))
		{
			newAppState = newAppState with
			{
				HomeSearch = null,
				HomeSearchInBookmarks = true,
				HomeSearchInSaves = true,
				HomeSearchInViews = true,
				HomeTags = new(),
				HomeBookmarks = new(),
				HomeSaves = new(),
				HomeViews = new(),
				HomeSearchResults = new()
			};
			return (newAppState,newAuxDataState);
		}

		//This temporary implementation
		var (bookmarks, saves) = await GetUserBookmarksAsync(currentUser.Id);

		List<TucTag> homeTags = await GetHomeTagsAsync(bookmarks, saves);
		List<TucBookmark> homeBookmarks = await GetHomeBookmarksAsync(bookmarks);
		List<TucBookmark> homeSaves = await GetHomeSavesAsync(saves);
		List<TucSpaceView> homeViews = await GetHomeViewsAsync();
		List<TucSearchResult> homeSearchResults = await GetHomeSearchResultsAsync(
			userId: currentUser.Id,
			search: routeState.Search,
			searchInBookmarks: routeState.SearchInBookmarks,
			searchInSaves: routeState.SearchInSaves,
			searchInViews: routeState.SearchInViews,
			bookmarks: bookmarks,
			saves: saves
		);

		newAppState = newAppState with
		{
			HomeSearch = routeState.Search,
			HomeSearchInBookmarks = routeState.SearchInBookmarks,
			HomeSearchInSaves = routeState.SearchInSaves,
			HomeSearchInViews = routeState.SearchInViews,
			HomeTags = homeTags,
			HomeBookmarks = homeBookmarks,
			HomeSaves = homeSaves,
			HomeViews = homeViews,
			HomeSearchResults = homeSearchResults
		};


		return (newAppState,newAuxDataState);
	}

	internal async Task<List<TucSearchResult>> GetHomeSearchResultsAsync(Guid userId, string search = null,
		bool searchInBookmarks = true, bool searchInSaves = true,
		bool searchInViews = true,
		List<TucBookmark> bookmarks = null, List<TucBookmark> saves = null)
	{
		var results = new List<TucSearchResult>();
		if (!String.IsNullOrWhiteSpace(search)) search = search.Trim().ToLowerInvariant();
		var allSpaces = await GetAllSpaces();
		var allSpaceViews = await GetAllSpaceViews();
		var spaceDict = allSpaces.ToDictionary(x => x.Id);
		var spaceViewDict = allSpaceViews.ToDictionary(x => x.Id);
		if (searchInViews)
		{
			foreach (var record in allSpaceViews)
			{
				if (!String.IsNullOrWhiteSpace(search)
						&& !record.Name.ToLowerInvariant().Contains(search))
					continue;
				var space = spaceDict[record.SpaceId];
				results.Add(new TucSearchResult(record, space));
			}
		}

		if (searchInBookmarks || searchInSaves)
		{
			if (bookmarks is null || saves is null)
				(bookmarks, saves) = await GetUserBookmarksAsync(userId);
			if (searchInBookmarks)
			{
				foreach (var record in bookmarks)
				{
					if (!String.IsNullOrWhiteSpace(search)
						&& !(record.Name.ToLowerInvariant().Contains(search) || record.Description.ToLowerInvariant().Contains(search)))
						continue;

					results.Add(new TucSearchResult(record, TucSearchResultType.Bookmark));
				}
			}
			if (searchInSaves)
			{
				foreach (var record in saves)
				{
					if (!String.IsNullOrWhiteSpace(search)
						&& !(record.Name.ToLowerInvariant().Contains(search) || record.Description.ToLowerInvariant().Contains(search)))
						continue;

					results.Add(new TucSearchResult(record, TucSearchResultType.UrlSave));
				}
			}
		}

		return results.OrderByDescending(x => x.CreatedOn).Take(TfConstants.HomeSearchMaxResults).ToList();
	}

	internal Task<List<TucTag>> GetHomeTagsAsync(List<TucBookmark> bookmarks, List<TucBookmark> saves)
	{
		//Temporary method for presenting tags
		var result = new List<TucTag>();
		var addedTagsHS = new HashSet<Guid>();
		foreach (var item in saves.OrderByDescending(x => x.CreatedOn))
		{
			foreach (var tag in item.Tags)
			{
				if (addedTagsHS.Contains(tag.Id)) continue;
				addedTagsHS.Add(tag.Id);
				result.Add(tag);
			}
		}
		foreach (var item in bookmarks.OrderByDescending(x => x.CreatedOn))
		{
			foreach (var tag in item.Tags)
			{
				if (addedTagsHS.Contains(tag.Id)) continue;
				addedTagsHS.Add(tag.Id);
				result.Add(tag);
			}
		}

		return Task.FromResult(result.Take(TfConstants.HomeTagsMaxResult).ToList());
	}

	internal Task<List<TucBookmark>> GetHomeBookmarksAsync(List<TucBookmark> bookmarks)
	{
		return Task.FromResult(bookmarks.OrderByDescending(x => x.CreatedOn).Take(TfConstants.HomeSubListsMaxResult).ToList());
	}

	internal Task<List<TucBookmark>> GetHomeSavesAsync(List<TucBookmark> saves)
	{
		return Task.FromResult(saves.OrderByDescending(x => x.CreatedOn).Take(TfConstants.HomeSubListsMaxResult).ToList());
	}

	internal async Task<List<TucSpaceView>> GetHomeViewsAsync()
	{
		return (await GetAllSpaceViews()).OrderByDescending(x => x.Position).Take(TfConstants.HomeSubListsMaxResult).ToList();

	}
}
