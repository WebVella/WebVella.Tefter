namespace WebVella.Tefter.UseCases.AppState;
internal partial class AppStateUseCase
{
	internal async Task<TfAppState> InitBookmarksAsync(TucUser currentUser, TfRouteState routeState, TfAppState newState, TfAppState oldState)
	{
		if (
			!(routeState.FirstNode == RouteDataFirstNode.Home
			|| routeState.FirstNode == RouteDataFirstNode.FastAccess
			|| routeState.FirstNode == RouteDataFirstNode.Space)
			)
		{
			newState = newState with { CurrentUserBookmarks = null, CurrentUserSaves = null };
			return newState;
		}
		if (newState.CurrentUserBookmarks == null || newState.CurrentUserSaves == null)
		{
			var (bookmarks, saves) = await GetUserBookmarksAsync(currentUser);
			newState = newState with { CurrentUserBookmarks = bookmarks, CurrentUserSaves = saves };
		}

		return newState;
	}

	internal async Task<(List<TucBookmark>, List<TucBookmark>)> GetUserBookmarksAsync(TucUser user)
	{

		var serviceResult = _spaceManager.GetUserBookmarksList(user.Id);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetUserBookmarksList failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return (new List<TucBookmark>(), new List<TucBookmark>());
		}
		var allSpaces = await GetAllSpaces();
		var allViews = await GetAllSpaceViews();
		var spaceDict = allSpaces.ToDictionary(x => x.Id);
		var viewDict = allViews.ToDictionary(x => x.Id);

		var bookmarks = new List<TucBookmark>();
		var saves = new List<TucBookmark>();

		foreach (var item in serviceResult.Value)
		{
			var record = new TucBookmark(item);
			if (viewDict.ContainsKey(record.SpaceViewId))
			{
				var spaceView = viewDict[record.SpaceViewId];
				var space = spaceDict[spaceView.SpaceId];

				record = record with
				{
					SpaceViewName = spaceView.Name,
					SpaceName = space.Name,
					SpaceColor = space.Color,
					SpaceIcon = space.Icon,
				};
			}
			if (String.IsNullOrWhiteSpace(item.Url))
				saves.Add(record);
			else
				bookmarks.Add(record);
		}

		return (bookmarks, saves);

	}

}
