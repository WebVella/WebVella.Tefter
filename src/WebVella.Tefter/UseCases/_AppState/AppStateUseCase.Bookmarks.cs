namespace WebVella.Tefter.UseCases.AppState;
internal partial class AppStateUseCase
{
	internal async Task<TfAppState> InitBookmarksAsync(IServiceProvider serviceProvider,
		TucUser currentUser, TfRouteState routeState,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
	{
		if (
			!(routeState.FirstNode == RouteDataFirstNode.Home
			|| routeState.FirstNode == RouteDataFirstNode.Space)
			)
		{
			newAppState = newAppState with
			{
				CurrentUserBookmarks = null,
				CurrentUserSaves = null,
				ActiveSpaceViewSavedUrl = null,
				ActiveSpaceViewBookmark = null
			};
			return newAppState;
		}
		if (newAppState.CurrentUserBookmarks == null || newAppState.CurrentUserSaves == null)
		{
			var (bookmarks, saves) = await GetUserBookmarksAsync(currentUser.Id);
			newAppState = newAppState with
			{
				CurrentUserBookmarks = bookmarks,
				CurrentUserSaves = saves
			};
		}
		var activeSave = newAppState.CurrentUserSaves.FirstOrDefault(x => x.Id == routeState.ActiveSaveId);
		var activeBookmark = newAppState.CurrentUserBookmarks.FirstOrDefault(x => x.SpaceViewId == routeState.SpaceViewId);
		newAppState = newAppState with
		{
			ActiveSpaceViewSavedUrl = activeSave,
			ActiveSpaceViewBookmark = activeBookmark
		};

		return newAppState;
	}

	internal async Task<(List<TucBookmark>, List<TucBookmark>)> GetUserBookmarksAsync(Guid userId)
	{

		var serviceResult = _spaceManager.GetUserBookmarksList(userId);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetUserBookmarksList failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage:"Invalid Data",
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
					SpaceId = space.Id,
				};
			}
			if (!String.IsNullOrWhiteSpace(item.Url))
				saves.Add(record);
			else
				bookmarks.Add(record);
		}

		return (bookmarks, saves);

	}

	internal async Task<Result<(List<TucBookmark>, List<TucBookmark>)>> CreateBookmarkAsync(TucBookmark bookmark)
	{
		var serviceResult = _spaceManager.CreateBookmark(bookmark.ToModel());
		if (serviceResult.IsFailed) return Result.Fail(new Error("CreateBookmark failed").CausedBy(serviceResult.Errors));
		return Result.Ok(await GetUserBookmarksAsync(bookmark.UserId));
	}

	internal async Task<Result<(List<TucBookmark>, List<TucBookmark>)>> UpdateBookmarkAsync(TucBookmark bookmark)
	{
		var serviceResult = _spaceManager.UpdateBookmark(bookmark.ToModel());
		if (serviceResult.IsFailed) return Result.Fail(new Error("UpdateBookmark failed").CausedBy(serviceResult.Errors));
		return Result.Ok(await GetUserBookmarksAsync(bookmark.UserId));
	}

	internal async Task<Result<(List<TucBookmark>, List<TucBookmark>)>> DeleteBookmarkAsync(TucBookmark bookmark)
	{
		var serviceResult = _spaceManager.DeleteBookmark(bookmark.Id);
		if (serviceResult.IsFailed) return Result.Fail(new Error("DeleteBookmark failed").CausedBy(serviceResult.Errors));

		return Result.Ok(await GetUserBookmarksAsync(bookmark.UserId));
	}

}
