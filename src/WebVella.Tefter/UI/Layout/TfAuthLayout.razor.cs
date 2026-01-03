using ITfEventBus = WebVella.Tefter.UI.EventsBus.ITfEventBus;

namespace WebVella.Tefter.UI.Layout;

public partial class TfAuthLayout : LayoutComponentBase, IAsyncDisposable
{
	[Inject] protected ITfEventBus TfEventBus { get; set; } = null!;
	[Inject] public ITfService TfService { get; set; } = null!;

	[Inject] protected NavigationManager Navigator { get; set; } = null!;
	[Inject] protected IJSRuntime JsRuntime { get; set; } = null!;
	[Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
	[Inject] protected IToastService ToastService { get; set; } = null!;

	private readonly Guid _sessionId = Guid.NewGuid();
	private TfState _state = new();
	private TfUser _currentUser = new();
	private bool _isLoaded = false;
	private string _urlInitialized = string.Empty;
	private IDisposable? _locationChangingHandler;

	private TfColor _accentColor = TfColor.Red500;
	private DesignThemeModes _themeMode = DesignThemeModes.System;

	private IAsyncDisposable _bookmarkEventSubscriber = null!;
	private IAsyncDisposable _spaceUpdatedEventSubscriber = null!;
	private IAsyncDisposable _userUpdatedEventSubscriber = null!;

	public TfState GetState() => _state;
	public Guid GetSessionId() => _sessionId;
	public Guid GetUserId() => _state.User.Id;

	public async ValueTask DisposeAsync()
	{
		await _bookmarkEventSubscriber.DisposeAsync();
		await _spaceUpdatedEventSubscriber.DisposeAsync();
		await _userUpdatedEventSubscriber.DisposeAsync();
		_locationChangingHandler?.Dispose();
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		//User checks
		var user = await TfService.GetUserFromCookieAsync(
			jsRuntime: JsRuntime,
			authStateProvider: AuthenticationStateProvider);

		if (user is null)
		{
			Navigator.NavigateTo(TfConstants.LoginPageUrl, true);
			return;
		}

		_currentUser = user;

		if (!_checkAccess(Navigator.Uri))
			Navigator.NavigateTo(string.Format(TfConstants.NoAccessPage));
		//init state
		await _init(Navigator.Uri);
		_urlInitialized = Navigator.Uri;
		_isLoaded = true;
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			_locationChangingHandler = Navigator.RegisterLocationChangingHandler(Navigator_LocationChanging);
			_bookmarkEventSubscriber = await TfEventBus.SubscribeAsync<TfBookmarkEventPayload>(
				handler: On_BookmarkEventAsync,
				matchKey: (key) => key == GetUserId().ToString());
			_spaceUpdatedEventSubscriber = await TfEventBus.SubscribeAsync<TfSpaceUpdatedEventPayload>(
				handler: On_SpaceUpdatedEventAsync,
				matchKey: (_) => true);
			_userUpdatedEventSubscriber = await TfEventBus.SubscribeAsync<TfUserUpdatedEventPayload>(
				handler: On_UserUpdatedEventAsync,
				matchKey: (_) => true);
		}
	}

	private async Task On_UserUpdatedEventAsync(string? key, TfUserUpdatedEventPayload? payload)
	{
		if(payload is null) return;
		if(payload.User.Id != _state.User.Id) return;
		_currentUser = payload.User;
		await _init(Navigator.Uri);
		await InvokeAsync(StateHasChanged);
	}		

	private async Task On_SpaceUpdatedEventAsync(string? key, TfSpaceUpdatedEventPayload? payload)
	{
		if(payload is null) return;
		if(payload.Space.Id != _state.Space?.Id) return;
		await _init(Navigator.Uri, payload.Space);
		await InvokeAsync(StateHasChanged);
	}

	private Task On_BookmarkEventAsync(string? key, TfBookmarkEventPayload? payload)
	{
		_state.UserBookmarks = TfService.GetBookmarksForUser(_currentUser.Id);
		return Task.CompletedTask;
	}


	private async ValueTask Navigator_LocationChanging(LocationChangingContext args)
	{
		if (_urlInitialized != args.TargetLocation)
		{
			if (!_checkAccess(args.TargetLocation))
			{
				ToastService.ShowError("Access Denied");
				args.PreventNavigation();
				return;
			}

			await _init(args.TargetLocation);
			_urlInitialized = args.TargetLocation;
		}
	}

	private async Task _init(string url, TfSpace? space = null)
	{
		_state = new TfState();
		try
		{
			_state = TfService.GetAppState(Navigator, _currentUser, url,
				String.IsNullOrWhiteSpace(_state.Uri) ? null : _state, space);
		}
		catch (Exception ex)
		{
			_isLoaded = false;
			await InvokeAsync(StateHasChanged);
			await Navigator.ToErrorPage(JsRuntime, ex.Message);
			return;
		}

		if (_state.NavigationState.RouteNodes.Count == 0) { }
		else if (_state.NavigationState.RouteNodes[0] == RouteDataNode.Home)
		{
			_accentColor = _currentUser.Settings.Color;
		}
		else if (_state.NavigationState.RouteNodes[0] == RouteDataNode.Space)
		{
			_accentColor = _state.Space?.Color ?? TfConstants.DefaultThemeColor;
		}
		else
		{
			_accentColor = TfColor.Amber500;
		}

		_themeMode = _currentUser.Settings.ThemeMode;
	}

	private bool _checkAccess(string url)
	{
		if (TfService.UserHasAccess(_currentUser, Navigator, url))
			return true;
		return false;
	}
}