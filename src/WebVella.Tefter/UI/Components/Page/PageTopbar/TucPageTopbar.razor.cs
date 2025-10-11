namespace WebVella.Tefter.UI.Components;

public partial class TucPageTopbar : TfBaseComponent, IDisposable
{
	private TfBookmark? _activeSavedUrl = null!;
	private bool _hasBookmark = false;
	private Icon _bookmarkIcon = null!;
	private string _bookmarkTitle = null!;
	private TucPageLinkSaveSelector _saveSelector = null!;
	private bool _userMenuVisible = false;

	public void Dispose()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
	}

	protected override void OnInitialized()
	{
		_init();
		Navigator.LocationChanged += On_NavigationStateChanged;
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		_init();
		StateHasChanged();
	}

	private void _init()
	{
		var state = TfAuthLayout.GetState();
		_activeSavedUrl = state.UserSaves.FirstOrDefault(x => x.Id == state.NavigationState.ActiveSaveId);
		_initBookmark();
	}

	private async Task _onSaveLinkClick()
	{
		if (TfAuthLayout.GetState().SpacePage is null)
			return;
		if (_activeSavedUrl is not null)
		{
			await _saveSelector.ToggleSelector();
			return;
		}

		try
		{
			var submit = new TfBookmark
			{
				Id = Guid.NewGuid(),
				SpaceId = TfAuthLayout.GetState().SpacePage.Id,
				UserId = TfAuthLayout.GetState().User.Id,
				CreatedOn = DateTime.Now,
				Description = String.Empty, //initially nothing is added for convenience
				Name = TfAuthLayout.GetState().SpacePage.Name + " " + DateTime.Now.ToString("dd-MM-yyyy HH:mm"),
				Url = new Uri(Navigator.Uri).PathAndQuery
			};
			TfService.CreateBookmark(submit);

			ToastService.ShowSuccess(LOC("URL is now saved"));
			await Navigator.ApplyChangeToUrlQuery(TfConstants.ActiveSaveQueryName, submit.Id);
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
	
	private async Task _themeChange()
	{
		var dialog = await DialogService.ShowDialogAsync<TucUserThemeDialog>(
			TfAuthLayout.GetState().User,
			new DialogParameters()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null) { }
	}	

	private async Task _changeLanguage()
	{
		var dialog = await DialogService.ShowDialogAsync<TucUserLanguageDialog>(
			TfAuthLayout.GetState().User,
			new DialogParameters()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null) { }
	}		
	
	private void _initBookmark()
	{
		var state = TfAuthLayout.GetState();
		_bookmarkIcon = TfConstants.GetIcon("Star")!;
		_bookmarkTitle = LOC("Bookmark this space");
		_hasBookmark = false;
		if (state.Space is not null && state.UserBookmarks.Any(x => x.Id == state.Space.Id))
		{
			_bookmarkIcon = TfConstants.GetIcon("Star", variant: IconVariant.Filled)!;
			_bookmarkTitle = LOC("Remove space bookmark");
			_hasBookmark = true;
		}
	}

	private async Task _onBookmarkClick()
	{
		var state = TfAuthLayout.GetState();
		if (state.Space is null) return;
		try
		{
			TfService.ToggleBookmark(
				userId: state.User.Id,
				spaceId: state.Space.Id
			);
			_initBookmark();
			ToastService.ShowSuccess(_hasBookmark ? LOC("Space Bookmarked") : LOC("Space bookmark removed"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}


	private async Task _setUrlAsStartup()
	{
		var uri = new Uri(Navigator.Uri);
		try
		{
			var user = await TfService.SetStartUpUrl(
				userId: TfAuthLayout.GetState().User.Id,
				url: uri.PathAndQuery
			);
			ToastService.ShowSuccess(LOC("Startup URL was successfully changed!"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}

	private async Task _resetUrlAsStartup()
	{
		try
		{
			var user = await TfService.SetStartUpUrl(
				userId: TfAuthLayout.GetState().User.Id,
				url: null
			);
			ToastService.ShowSuccess(LOC("Startup URL was successfully changed!"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}


	private bool _isSetAsStartupUri()
	{
		var uri = new Uri(Navigator.Uri);
		return uri.PathAndQuery == TfAuthLayout.GetState().User.Settings.StartUpUrl;
	}

	private async Task _logout()
	{
		await TfService.LogoutAsync(JSRuntime);
		Navigator.NavigateTo(TfConstants.LoginPageUrl, true);
	}
}