namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	public Task<TfAdminDashboardData> GetAdminDashboardData();
	public TfHomeDashboardData GetHomeDashboardData(Guid userId,
		string? search = null,
		bool searchInBookmarks = true,
		bool searchInSaves = true,
		bool searchInViews = true);
}

public partial class TfService : ITfService
{
	public Task<TfAdminDashboardData> GetAdminDashboardData()
	{
		try
		{
			var result = new TfAdminDashboardData();
			result.ProvidersInfo = GetDataProvidersInfo();
			result.SyncInfo = result.ProvidersInfo.Where(x => x.NextSyncOn is not null).OrderBy(x => x.NextSyncOn).Take(5).ToList();

			return Task.FromResult(result);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfHomeDashboardData GetHomeDashboardData(Guid userId,
		string? search = null,
		bool searchInBookmarks = true,
		bool searchInSaves = true,
		bool searchInViews = true)
	{
		try
		{
			var result = new TfHomeDashboardData();
			var providers = GetDataProviders();
			var spaces = GetSpacesList();
			var spacePages = GetAllSpacePages();
			var (bookmarks, saves) = GetUserBookmarksAsync(userId);
			List<TfTag> homeTags = GetHomeTagsAsync(bookmarks, saves);
			List<TfBookmark> homeBookmarks = GetHomeBookmarksAsync(bookmarks);
			List<TfBookmark> homeSaves = GetHomeBookmarksAsync(saves);
			List<TfSpaceView> homeViews = GetHomeViewsAsync();
			List<TfSearchResult> homeSearchResults = GetHomeSearchResultsAsync(
				userId: userId,
				search: search,
				searchInBookmarks: searchInBookmarks,
				searchInSaves: searchInSaves,
				searchInViews: searchInViews,
				bookmarks: bookmarks,
				saves: saves
			);

			result.HomeTags = homeTags;
			result.HomeBookmarks = homeBookmarks;
			result.HomeSaves = homeSaves;
			result.HomeViews = homeViews;
			result.HomeSearchResults = homeSearchResults;
			result.ProvidersCount = providers.Count;
			result.SpacesCount = spaces.Count;
			result.SpacePagesCount = spacePages.Count;


			return result;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public List<TfSearchResult> GetHomeSearchResultsAsync(Guid userId, string? search = null,
			bool searchInBookmarks = true, bool searchInSaves = true,
			bool searchInViews = true,
			List<TfSearchResult>? bookmarks = null, List<TfSearchResult>? saves = null)
	{
		var results = new List<TfSearchResult>();
		if (!String.IsNullOrWhiteSpace(search)) search = search.Trim().ToLowerInvariant();
		var allSpaces = GetSpacesList();
		var allSpaceViews = GetAllSpaceViews();
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
				results.Add(new TfSearchResult
				{
					Id = record.Id,
					Title = record.Name,
					Description = String.Empty,
					Type = TfSearchResultType.SpaceView,
					CreatedOn = null,
					SpaceViewId = record.Id,
					SpaceId = record.SpaceId,
					SpaceViewName = record.Name,
					SpaceName = space.Name,
					SpaceColor = space.Color ?? TfConstants.DefaultThemeColor,
					SpaceIcon = space.FluentIconName,
					Url = string.Format(TfConstants.SpaceViewPageUrl, record.SpaceId, record.Id)
				});
			}
		}

		if (searchInBookmarks || searchInSaves)
		{
			if (bookmarks is null || saves is null)
				(bookmarks, saves) = GetUserBookmarksAsync(userId);
			if (searchInBookmarks)
			{
				foreach (var record in bookmarks)
				{
					if (!String.IsNullOrWhiteSpace(search)
						&& (String.IsNullOrWhiteSpace(record.Title) || !record.Title.ToLowerInvariant().Contains(search))
						&& (String.IsNullOrWhiteSpace(record.Description) || !record.Description.ToLowerInvariant().Contains(search)))
						continue;

					results.Add(record);
				}
			}
			if (searchInSaves)
			{
				foreach (var record in saves)
				{
					if (!String.IsNullOrWhiteSpace(search)
						&& (String.IsNullOrWhiteSpace(record.Title) || !record.Title.ToLowerInvariant().Contains(search))
						&& (String.IsNullOrWhiteSpace(record.Description) || !record.Description.ToLowerInvariant().Contains(search)))
						continue;

					results.Add(record);
				}
			}
		}

		return results.OrderByDescending(x => x.CreatedOn).Take(TfConstants.HomeSearchMaxResults).ToList();
	}

	private (List<TfSearchResult>, List<TfSearchResult>) GetUserBookmarksAsync(
			Guid userId)
	{
		var userBookmarks = GetBookmarksListForUser(userId);
		var allSpaces = GetSpacesList();
		var allViews = GetAllSpaceViews();
		var spaceDict = allSpaces.ToDictionary(x => x.Id);
		var viewDict = allViews.ToDictionary(x => x.Id);

		var bookmarks = new List<TfSearchResult>();
		var saves = new List<TfSearchResult>();

		foreach (var item in userBookmarks)
		{
			if (viewDict.ContainsKey(item.SpaceId))
			{
				var spaceView = viewDict[item.SpaceId];
				var space = spaceDict[spaceView.SpaceId];
				var record = new TfSearchResult()
				{
					CreatedOn = item.CreatedOn,
					Description = item.Description,
					Id = item.Id,
					SpaceColor = space.Color ?? TfConstants.DefaultThemeColor,
					SpaceIcon = space.FluentIconName,
					SpaceId = space.Id,
					SpaceName = space.Name,
					SpaceViewId = spaceView.Id,
					SpaceViewName = spaceView.Name,
					Title = item.Name,
					Type = TfSearchResultType.Bookmark,
					Tags = item.Tags,
					Url = item.Url
				};
				if (!String.IsNullOrWhiteSpace(item.Url))
				{
					record.Type = TfSearchResultType.UrlSave;
					saves.Add(record);
				}
				else
					bookmarks.Add(record);
			}

		}

		return (bookmarks, saves);

	}

	private List<TfTag> GetHomeTagsAsync(List<TfSearchResult> bookmarks, List<TfSearchResult> saves)
	{
		//Temporary method for presenting tags
		var result = new List<TfTag>();
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

		return result.Take(TfConstants.HomeTagsMaxResult).ToList();
	}

	private List<TfBookmark> GetHomeBookmarksAsync(List<TfSearchResult> bookmarks)
	{
		var result = new List<TfBookmark>();
		foreach (var item in bookmarks.OrderByDescending(x => x.CreatedOn).Take(TfConstants.HomeSubListsMaxResult))
		{
			result.Add(new TfBookmark
			{
				Id = item.Id,
				CreatedOn = item.CreatedOn ?? DateTime.Now,
				Description = item.Description ?? String.Empty,
				Name = item.Title ?? String.Empty,
				SpaceId = item.SpaceViewId ?? Guid.Empty,
				Tags = item.Tags,
				Url = item.Url ?? String.Empty
			});
		}
		return result;
	}

	private List<TfSpaceView> GetHomeViewsAsync()
	{
		return GetAllSpaceViews().OrderByDescending(x => x.Position).Take(TfConstants.HomeSubListsMaxResult).ToList();

	}
}
