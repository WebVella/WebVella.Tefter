namespace WebVella.Tefter.UI.Components;

public partial class TucPageTopbarBookmarks : TfBaseComponent, IAsyncDisposable
{
	private List<TfBookmark> _allUserBookmarks = new();

	private TfBookmark? _activePin = null;
	private readonly string _pinBtnId = "tf-page-top-pin-selector";
	private readonly string _bookmarkBtnId = "tf-page-top-bookmark";
	
	private TfBookmark? _activeBookmark = null;
	private bool _bookmarkMenuOpen = false;

	private IAsyncDisposable _bookmarkEventSubscriber = null!;

	#region << Lifecycle >>

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		await _bookmarkEventSubscriber.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		_init(forceInit: false);
		Navigator.LocationChanged += On_NavigationStateChanged;
		_bookmarkEventSubscriber = await TfEventBus.SubscribeAsync<TfBookmarkEventPayload>(
			handler:On_BookmarkEventAsync, 
			matchKey: (key) => key == TfAuthLayout.GetUserId().ToString());
	}

	#endregion

	#region << Event Listeners >>
	private async Task On_BookmarkEventAsync(string? key, TfBookmarkEventPayload? payload)
	{
		_init(forceInit: true);
		await InvokeAsync(StateHasChanged);
	}
	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		_init(forceInit: true);
		StateHasChanged();
	}

	#endregion

	private void _init(bool forceInit = false)
	{
		_allUserBookmarks = new();
		var state = TfAuthLayout.GetState();
		if (forceInit)
		{
			_allUserBookmarks = TfService.GetBookmarksForUser(state.User.Id);
		}
		else
		{
			_allUserBookmarks.AddRange(state.UserBookmarks);
		}

		_activePin = null;
		if (state.SpacePage is not null)
			_activePin = _allUserBookmarks.FirstOrDefault(x =>
				x.SpacePageId == state.SpacePage.Id && String.IsNullOrWhiteSpace(x.Url));
		_activeBookmark = null;

		if (state.NavigationState.ActiveSaveId is not null)
			_activeBookmark = _allUserBookmarks.FirstOrDefault(x => x.Id == state.NavigationState.ActiveSaveId.Value);
	}

	#region << Pin >>

	private async Task _onPinClick()
	{
		var state = TfAuthLayout.GetState();
		if (state.SpacePage is null) return;

		try
		{
			if (_activePin is null)
			{
				var bookmark = new TfBookmark
				{
					Id = Guid.NewGuid(),
					UserId = state.User.Id,
					SpacePageId = state.SpacePage.Id,
					Name = state.SpacePage.Name,
					Description = state.SpacePage.Description ?? String.Empty,
					Type = TfBookmarkType.Page
				};
				var result = TfService.CreateBookmark(bookmark);
				ToastService.ShowSuccess(LOC("Page Pinned"));
				await TfEventBus.PublishAsync(
					key: TfAuthLayout.GetUserId(),
					payload: new TfBookmarkCreatedEventPayload(result));				
			}
			else
			{
				TfService.DeleteBookmark(_activePin.Id);
				ToastService.ShowSuccess(LOC("Page unpinned"));
				await TfEventBus.PublishAsync(
					key: TfAuthLayout.GetUserId(),
					payload: new TfBookmarkDeletedEventPayload(_activePin!));						
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}

	#endregion

	#region << Bookmark >>

	private async Task _onBookmarkClick()
	{
		var state = TfAuthLayout.GetState();
		if (state.SpacePage is null) return;

		if (_activeBookmark is null)
		{
			try
			{
				var bookmark = new TfBookmark
				{
					Id = Guid.NewGuid(),
					UserId = state.User.Id,
					SpacePageId = state.SpacePage.Id,
					Name =
						state.SpacePage.Name,
					Description = state.SpacePage.Description ?? String.Empty,
					Url = new Uri(Navigator.Uri).PathAndQuery,
					Type = TfBookmarkType.URL
				};
				var result = TfService.CreateBookmark(bookmark);
				ToastService.ShowSuccess(LOC("URL Saved"));
				await TfEventBus.PublishAsync(
					key: TfAuthLayout.GetUserId(),
					payload: new TfBookmarkCreatedEventPayload(result));				
				await Navigator.ApplyChangeToUrlQuery(TfConstants.ActiveSaveQueryName, bookmark.Id);
			}
			catch (Exception ex)
			{
				ProcessException(ex);
			}
		}
		else
		{
			_bookmarkMenuOpen = true;
		}
	}

	private async Task _bookmarkEdit()
	{
		if (_activeBookmark is null) return;

		var dialog = await DialogService.ShowDialogAsync<TucSpaceViewBookmarkManageDialog>(
			_activeBookmark,
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		_ = await dialog.Result;
	}

	private async Task _bookmarkUpdateUrl()
	{
		if (_activeBookmark is null) return;

		try
		{
			var submit = _activeBookmark with { Url = new Uri(Navigator.Uri).PathAndQuery };
			var result = TfService.UpdateBookmark(submit);
			ToastService.ShowSuccess(LOC("URL updated"));
			await TfEventBus.PublishAsync(
				key: TfAuthLayout.GetUserId(),
				payload: new TfBookmarkUpdatedEventPayload(result));					
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _bookmarkRemove()
	{
		try
		{
			if (_activeBookmark is null)
				return;

			TfService.DeleteBookmark(_activeBookmark.Id);
			await TfEventBus.PublishAsync(
				key: TfAuthLayout.GetUserId(),
				payload: new TfBookmarkDeletedEventPayload(_activeBookmark!));				
			ToastService.ShowSuccess(LOC("Save URL removed"));
			await Navigator.ApplyChangeToUrlQuery(TfConstants.ActiveSaveQueryName, null);
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			await InvokeAsync(StateHasChanged);
		}
	}

	private Color _getLinkColor()
	{
		if (_activeBookmark is null || String.IsNullOrWhiteSpace(_activeBookmark.Url)) return Color.Neutral;
		if (Navigator.IsSpaceViewSavedUrlChanged(_activeBookmark.Url))
			return Color.Warning;

		return Color.Neutral;
	}

	#endregion
}