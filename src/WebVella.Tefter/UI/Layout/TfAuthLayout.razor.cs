using Microsoft.FluentUI.AspNetCore.Components.DesignTokens;

namespace WebVella.Tefter.UI.Layout;

public partial class TfAuthLayout : LayoutComponentBase, IAsyncDisposable
{
	[Inject] public ITfService TfService { get; set; } = null!;
	[Inject] protected ITfConfigurationService TfConfigurationService { get; set; } = null!;
	[Inject] protected TfGlobalEventProvider TfEventProvider { get; set; } = null!;
	[Inject] protected NavigationManager Navigator { get; set; } = null!;
	[Inject] protected IJSRuntime JsRuntime { get; set; } = null!;
	[Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
	[Inject] protected IToastService ToastService { get; set; } = null!;

	private TfState _state = new();
	private TfUser _currentUser = new();
	private bool _isLoaded = false;
	private string _urlInitialized = string.Empty;
	private IDisposable? _locationChangingHandler;

	private TfColor _accentColor = TfColor.Red500;
	private DesignThemeModes _themeMode = DesignThemeModes.System;


	public TfState GetState() => _state;

	public ValueTask DisposeAsync()
	{
		TfEventProvider.Dispose();
		_locationChangingHandler?.Dispose();
		return ValueTask.CompletedTask;
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
		_init(Navigator.Uri);
		_urlInitialized = Navigator.Uri;
		_isLoaded = true;
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			_locationChangingHandler = Navigator.RegisterLocationChangingHandler(Navigator_LocationChanging);
			TfEventProvider.UserUpdatedEvent += On_UserUpdated;
			TfEventProvider.SpaceUpdatedEvent += On_SpaceUpdated;
		}
	}

	private async Task On_UserUpdated(TfUserUpdatedEvent args)
	{
		await InvokeAsync(async () =>
		{
			if (args.Payload.Id == _state.User.Id)
			{
				_currentUser = args.Payload;
				_init(Navigator.Uri);
				await InvokeAsync(StateHasChanged);
			}
		});
	}

	private async Task On_SpaceUpdated(TfSpaceUpdatedEvent args)
	{
		await InvokeAsync(async () =>
		{
			if (args.Payload.Id == _state.Space?.Id)
			{
				_init(Navigator.Uri, args.Payload);
				await InvokeAsync(StateHasChanged);
			}
		});
	}

	private ValueTask Navigator_LocationChanging(LocationChangingContext args)
	{
		if (_urlInitialized != args.TargetLocation)
		{
			if (!_checkAccess(args.TargetLocation))
			{
				ToastService.ShowError("Access Denied");
				args.PreventNavigation();
				return ValueTask.CompletedTask;
			}

			_init(args.TargetLocation);
			_urlInitialized = args.TargetLocation;
		}

		return ValueTask.CompletedTask;
	}

	private void _init(string url, TfSpace? space = null)
	{
		if (String.IsNullOrWhiteSpace(_state.Uri))
			_state = TfService.GetAppState(Navigator, _currentUser, url, null, space);
		else
			_state = TfService.GetAppState(Navigator, _currentUser, url, _state, space);

		if (_state.NavigationState.RouteNodes[0] == RouteDataNode.Home)
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
		_themeMode = _currentUser?.Settings?.ThemeMode ?? DesignThemeModes.System;
	}

	private bool _checkAccess(string url)
	{
		if (TfService.UserHasAccess(_currentUser, Navigator, url))
			return true;
		return false;
	}
}