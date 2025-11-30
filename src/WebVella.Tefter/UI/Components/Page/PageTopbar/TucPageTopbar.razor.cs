namespace WebVella.Tefter.UI.Components;

public partial class TucPageTopbar : TfBaseComponent, IAsyncDisposable
{
	[Inject] protected TfGlobalEventProvider TfEventProvider { get; set; } = null!;
	private DotNetObjectReference<TucPageTopbar> _objectRef = null!;
	private TfBookmark? _activeSavedUrl = null!;
	private bool _hasBookmark = false;
	private bool _userMenuVisible = false;

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		TfEventProvider.BookmarkCreatedEvent -= On_BookmarkCreatedEvent;
		TfEventProvider.BookmarkUpdatedEvent -= On_BookmarkUpdatedEvent;
		TfEventProvider.BookmarkDeletedEvent -= On_BookmarkDeletedEvent;		
		await TfEventProvider.DisposeAsync();
		try
		{
			await JSRuntime.InvokeAsync<object>("Tefter.removeThemeSwitchListener", ComponentId.ToString());
		}
		catch
		{
			//In rare ocasions the item is disposed after the JSRuntime is no longer avaible
		}
		_objectRef.Dispose();		
	}

	protected override async Task OnInitializedAsync()
	{
		_init();
		_objectRef = DotNetObjectReference.Create(this);
		await JSRuntime.InvokeAsync<object>(
			"Tefter.addThemeSwitchListener", _objectRef, ComponentId.ToString(), "OnThemeSwitchHandler");		
		Navigator.LocationChanged += On_NavigationStateChanged;
		TfEventProvider.BookmarkCreatedEvent += On_BookmarkCreatedEvent;
		TfEventProvider.BookmarkUpdatedEvent += On_BookmarkUpdatedEvent;
		TfEventProvider.BookmarkDeletedEvent += On_BookmarkDeletedEvent;
	}

	private async Task On_BookmarkCreatedEvent(TfBookmarkCreatedEvent args)
	{
		if (args.IsUserApplicable(this))
		{
			var state = TfAuthLayout.GetState();
			if(args.Payload.UserId != state.User.Id) return;
			if (String.IsNullOrWhiteSpace(args.Payload.Url)
			    && args.Payload.SpacePageId == state.SpacePage?.Id)
				_hasBookmark = true;
			//SAVE LINK -> URL will change after create
			await InvokeAsync(StateHasChanged);
		}
	}	
	
	private async Task On_BookmarkUpdatedEvent(TfBookmarkUpdatedEvent args)
	{
		if (args.IsUserApplicable(this))
		{
			var state = TfAuthLayout.GetState();
			if(args.Payload.UserId != state.User.Id) return;
			if (String.IsNullOrWhiteSpace(args.Payload.Url)
			    && args.Payload.SpacePageId == state.SpacePage?.Id)
				_hasBookmark = true;
			else if (args.Payload.Id == _activeSavedUrl?.Id)
				_activeSavedUrl = args.Payload;				
			await InvokeAsync(StateHasChanged);
		}
	}			
	
	private async Task On_BookmarkDeletedEvent(TfBookmarkDeletedEvent args)
	{
		if (args.IsUserApplicable(this))
		{
			var state = TfAuthLayout.GetState();
			if(args.Payload.UserId != state.User.Id) return;
			if (String.IsNullOrWhiteSpace(args.Payload.Url)
			    && args.Payload.SpacePageId == state.SpacePage?.Id)
				_hasBookmark = false;
			//SAVE LINK -> URL will change after delete
			await InvokeAsync(StateHasChanged);
		}
	}		
	
	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		_init();
		StateHasChanged();
	}

	private void _init()
	{
		var state = TfAuthLayout.GetState();
		_activeSavedUrl = null;
		if(state.NavigationState.ActiveSaveId is not null)
			_activeSavedUrl = TfService.GetBookmark(state.NavigationState.ActiveSaveId.Value);

		_hasBookmark = false;
		if (state.SpacePage is not null && state.UserBookmarks.Any(x => x.SpacePageId == state.SpacePage.Id))
		{
			_hasBookmark = true;
		}
	}

	private async Task _onSaveLinkClick()
	{
		var state = TfAuthLayout.GetState();
		if (state.SpacePage is null)
			return;
	
		try
		{
			if (_activeSavedUrl is not null)
			{
				//Remove
				if(!Navigator.IsSpaceViewSavedUrlChanged(_activeSavedUrl.Url))
				{
					TfService.DeleteBookmark(_activeSavedUrl.Id);
					ToastService.ShowSuccess(LOC("Saved URL removed"));
					await Navigator.ApplyChangeToUrlQuery(TfConstants.ActiveSaveQueryName, null);
				}
				//Update
				else
				{
					var submit = _activeSavedUrl with { Url = new Uri(Navigator.Uri).PathAndQuery };
					TfService.UpdateBookmark(submit);	
					ToastService.ShowSuccess(LOC("URL updated"));
				}
			}
			//Create
			else
			{

				var submit = new TfBookmark
				{
					Id = Guid.NewGuid(),
					SpacePageId = TfAuthLayout.GetState().SpacePage!.Id,
					UserId = TfAuthLayout.GetState().User.Id,
					CreatedOn = DateTime.Now,
					Description = String.Empty, //initially nothing is added for convenience
					Name = TfAuthLayout.GetState().SpacePage!.Name + " " + DateTime.Now.ToString("dd-MM-yyyy HH:mm"),
					Url = new Uri(Navigator.Uri).PathAndQuery
				};
				TfService.CreateBookmark(submit);

				ToastService.ShowSuccess(LOC("URL is now saved"));
				await Navigator.ApplyChangeToUrlQuery(TfConstants.ActiveSaveQueryName, submit.Id);
			}
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
	
	private async Task _visualPreferencesHandler()
	{
		var dialog = await DialogService.ShowDialogAsync<TucUserVisualPreferencesDialog>(
			TfAuthLayout.GetState().User,
			new ()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				TrapFocus = false
			});
		_ = await dialog.Result;
	}	

	private Task _onBookmarkClick()
	{
		var state = TfAuthLayout.GetState();
		if (state.SpacePage is null) return Task.CompletedTask;
		try
		{
			TfService.ToggleBookmark(
				userId: state.User.Id,
				spacePageId: state.SpacePage.Id
			);
			ToastService.ShowSuccess(_hasBookmark ? LOC("Page Bookmarked") : LOC("Page bookmark removed"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		return Task.CompletedTask;
	}


	private async Task _setUrlAsStartup()
	{
		var uri = new Uri(Navigator.Uri);
		try
		{
			_ = await TfService.SetStartUpUrl(
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
			_ = await TfService.SetStartUpUrl(
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
	
	[JSInvokable("OnThemeSwitchHandler")]
	public async Task OnGlobalSearchHandler()
	{
		try
		{
			DesignThemeModes newMode = DesignThemeModes.Light;
			if(TfAuthLayout.GetState().User.Settings.ThemeMode == DesignThemeModes.Light)
				newMode = DesignThemeModes.Dark;
			
			_ = await TfService.SetUserTheme(
				userId: TfAuthLayout.GetState().User.Id,
				themeMode: newMode
			);
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}		
}