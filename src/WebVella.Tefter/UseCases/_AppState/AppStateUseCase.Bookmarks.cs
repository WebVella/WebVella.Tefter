﻿namespace WebVella.Tefter.UseCases.AppState;
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
			newState = newState with { CurrentUserBookmarks = null, CurrentUserSaves = null, 
				ActiveSpaceViewSavedUrl = null, ActiveSpaceViewBookmark = null };
			return newState;
		}
		if (newState.CurrentUserBookmarks == null || newState.CurrentUserSaves == null)
		{
			var (bookmarks, saves) = await GetUserBookmarksAsync(currentUser.Id);
			newState = newState with
			{
				CurrentUserBookmarks = bookmarks,
				CurrentUserSaves = saves
			};
		}
		var activeSave = newState.CurrentUserSaves.FirstOrDefault(x=> x.Id == routeState.ActiveSaveId);
		var activeBookmark = newState.CurrentUserBookmarks.FirstOrDefault(x=> x.SpaceViewId == routeState.SpaceViewId);
		newState = newState with
		{
			ActiveSpaceViewSavedUrl = activeSave,
			ActiveSpaceViewBookmark = activeBookmark
		};

		return newState;
	}

	internal async Task<(List<TucBookmark>, List<TucBookmark>)> GetUserBookmarksAsync(Guid userId)
	{

		var serviceResult = _spaceManager.GetUserBookmarksList(userId);
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
