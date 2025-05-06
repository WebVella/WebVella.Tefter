namespace WebVella.Tefter.Web.Components;

public partial class TfThemeDefinition : TfBaseComponent
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Parameter] public DesignThemeModes? Mode { get; set; } = null;

	private string _themeStyles = "";
	private DesignThemeModes _mode = DesignThemeModes.System;

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await _initThemeStyles();
		EnableRenderLock();

	}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			ActionSubscriber.SubscribeToAction<SetUserStateAction>(this, On_UserStateChanged);
		}
	}

	private void On_UserStateChanged(SetUserStateAction action)
	{
		InvokeAsync(async () =>
		{
			await _initThemeStyles();
			RegenRenderLock();
			await InvokeAsync(StateHasChanged);
		});
	}

	private async Task _initThemeStyles()
	{
		var cacheKey = ConfigurationService.CacheKey;
		if (Mode is not null)
		{
			_mode = Mode.Value;
		}
		else
		{
			_mode = DesignThemeModes.Dark;
			if (TfUserState.Value.CurrentUser is not null && TfUserState.Value.CurrentUser.Settings is not null)
			{
				_mode = TfUserState.Value.CurrentUser.Settings.ThemeMode;
			}
			if (_mode == DesignThemeModes.System)
			{
				try
				{
					var browserModeIsDark = await _getIsDarkModeFromBrowser();
					_mode = browserModeIsDark ? DesignThemeModes.Dark : DesignThemeModes.Light;
				}
				catch
				{
					//there is a strange error here that happens when the browser stays opened for long
				}
			}
		}
		var sb = new StringBuilder();
		//Dark
		if (_mode == DesignThemeModes.Dark)
		{
			sb.AppendLine($"<link rel=\"stylesheet\" href=\"_content/WebVella.Tefter/dark.css?cb={cacheKey}\">");
		}
		//Light
		else
		{
			sb.AppendLine($"<link rel=\"stylesheet\" href=\"_content/WebVella.Tefter/light.css?cb={cacheKey}\">");
		}
		_themeStyles = sb.ToString();
	}

	private async Task<bool> _getIsDarkModeFromBrowser()
	{
		return await JSRuntime.InvokeAsync<bool>(
				"Tefter.getIsDarkTheme");
	}
}
