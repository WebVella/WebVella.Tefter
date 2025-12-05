namespace WebVella.Tefter.UI.Components;

public partial class TucPageTopbarBookmarks : TfBaseComponent, IAsyncDisposable
{
	[Inject] protected TfGlobalEventProvider TfEventProvider { get; set; } = null!;

	private List<TfBookmark> _allUserBookmarks = new();

	private TfBookmark? _activePin = null;
	private string _pinBtnId = "tf-page-top-pin-selector";

	private TfBookmark? _activeBookmark = null;
	private bool _bookmarkMenuOpen = false;
	private string _bookmarkBtnId = "tf-page-top-bookmark";

	#region << Lifecycle >>
	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		TfEventProvider.BookmarkCreatedEvent -= On_BookmarkCreatedEvent;
		TfEventProvider.BookmarkUpdatedEvent -= On_BookmarkUpdatedEvent;
		TfEventProvider.BookmarkDeletedEvent -= On_BookmarkDeletedEvent;
		await TfEventProvider.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		_init(forceInit: false);
		Navigator.LocationChanged += On_NavigationStateChanged;
		TfEventProvider.BookmarkCreatedEvent += On_BookmarkCreatedEvent;
		TfEventProvider.BookmarkUpdatedEvent += On_BookmarkUpdatedEvent;
		TfEventProvider.BookmarkDeletedEvent += On_BookmarkDeletedEvent;
	}

	#endregion

	#region << Event Listeners >>

	private async Task On_BookmarkCreatedEvent(TfBookmarkCreatedEvent args)
	{
		if (args.IsUserApplicable(this))
		{
			_init(forceInit: true);
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task On_BookmarkUpdatedEvent(TfBookmarkUpdatedEvent args)
	{
		if (args.IsUserApplicable(this))
		{
			_init(forceInit: true);
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task On_BookmarkDeletedEvent(TfBookmarkDeletedEvent args)
	{
		if (args.IsUserApplicable(this))
		{
			_init(forceInit: true);
			await InvokeAsync(StateHasChanged);
		}
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
			_activePin = _allUserBookmarks.FirstOrDefault(x => x.SpacePageId == state.SpacePage.Id && String.IsNullOrWhiteSpace(x.Url));
		_activeBookmark = null;

		if (state.NavigationState.ActiveSaveId is not null)
			_activeBookmark = _allUserBookmarks.FirstOrDefault(x => x.Id == state.NavigationState.ActiveSaveId.Value);


	}

	#region << Pin >>
	private Task _onPinClick()
	{
		var state = TfAuthLayout.GetState();
		if (state.SpacePage is null) return Task.CompletedTask;

		try
		{
			if (_activePin is null)
			{

				var bookmark = new TfBookmark
				{
					Id = Guid.NewGuid(),
					UserId = state.User.Id,
					SpacePageId = state.SpacePage.Id,
					Name = state.SpacePage.Name!,
					Description = state.SpacePage.Description ?? String.Empty
				};
				TfService.CreateBookmark(bookmark);
				ToastService.ShowSuccess(LOC("Page Pinned"));

			}
			else
			{
				TfService.DeleteBookmark(_activePin.Id);
				ToastService.ShowSuccess(LOC("Page unpinned"));
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		return Task.CompletedTask;
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
					Name = state.SpacePage.Name ?? "unknown space" + " " + DateTime.Now.ToString("dd-MM-yyyy HH:mm"),
					Description = state.SpacePage.Description ?? String.Empty,
					Url = new Uri(Navigator.Uri).PathAndQuery
				};
				TfService.CreateBookmark(bookmark);
				ToastService.ShowSuccess(LOC("URL Saved"));
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
			TfService.UpdateBookmark(submit);
			ToastService.ShowSuccess(LOC("URL updated"));
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

	private Microsoft.FluentUI.AspNetCore.Components.Color _getLinkColor()
	{
		if (_activeBookmark is null) return Microsoft.FluentUI.AspNetCore.Components.Color.Neutral;
		if (Navigator.IsSpaceViewSavedUrlChanged(_activeBookmark.Url))
			return Microsoft.FluentUI.AspNetCore.Components.Color.Warning;

		return Microsoft.FluentUI.AspNetCore.Components.Color.Neutral;
	}

	#endregion
}