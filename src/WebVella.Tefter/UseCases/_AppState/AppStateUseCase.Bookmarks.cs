namespace WebVella.Tefter.UseCases.AppState;
internal partial class AppStateUseCase
{
	internal async Task<(TfAppState, TfAuxDataState)> InitBookmarksAsync(IServiceProvider serviceProvider,
		TucUser currentUser, 
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
	{
		if (
			!(newAppState.Route.FirstNode == RouteDataFirstNode.Home
			|| newAppState.Route.FirstNode == RouteDataFirstNode.Space)
			)
		{
			newAppState = newAppState with
			{
				CurrentUserBookmarks = null,
				CurrentUserSaves = null,
				ActiveSpaceViewSavedUrl = null,
				ActiveSpaceViewBookmark = null
			};
			return (newAppState,newAuxDataState);
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
		var activeSave = newAppState.CurrentUserSaves.FirstOrDefault(x => x.Id == newAppState.Route.ActiveSaveId);
		var activeBookmark = newAppState.CurrentUserBookmarks.FirstOrDefault(x => x.SpaceViewId == newAppState.Route.SpaceViewId);
		newAppState = newAppState with
		{
			ActiveSpaceViewSavedUrl = activeSave,
			ActiveSpaceViewBookmark = activeBookmark
		};

		return (newAppState,newAuxDataState);
	}

	internal virtual async Task<(List<TucBookmark>, List<TucBookmark>)> GetUserBookmarksAsync(
		Guid userId)
	{
		try
		{
			var userBookmarks = _spaceManager.GetBookmarksListForUser(userId);
			var allSpaces = await GetAllSpaces();
			var allViews = await GetAllSpaceViews();
			var spaceDict = allSpaces.ToDictionary(x => x.Id);
			var viewDict = allViews.ToDictionary(x => x.Id);

			var bookmarks = new List<TucBookmark>();
			var saves = new List<TucBookmark>();

			foreach (var item in userBookmarks)
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
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceResult(
				exception: ex,
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return (new List<TucBookmark>(), new List<TucBookmark>());
		}
	}

	internal virtual async Task<(List<TucBookmark>, List<TucBookmark>)> CreateBookmarkAsync(
		TucBookmark bookmark)
	{
		_spaceManager.CreateBookmark(bookmark.ToModel());
		return await GetUserBookmarksAsync(bookmark.UserId);
	}

	internal virtual async Task<(List<TucBookmark>, List<TucBookmark>)> UpdateBookmarkAsync(
		TucBookmark bookmark)
	{
		_spaceManager.UpdateBookmark(bookmark.ToModel());
		return await GetUserBookmarksAsync(bookmark.UserId);
	}

	internal virtual async Task<(List<TucBookmark>, List<TucBookmark>)> DeleteBookmarkAsync(
		TucBookmark bookmark)
	{
		_spaceManager.DeleteBookmark(bookmark.Id);
		return await GetUserBookmarksAsync(bookmark.UserId);
	}

}
