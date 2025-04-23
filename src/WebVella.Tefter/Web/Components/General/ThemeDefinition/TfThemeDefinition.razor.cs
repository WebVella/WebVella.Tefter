namespace WebVella.Tefter.Web.Components;

public partial class TfThemeDefinition : TfBaseComponent
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }

	private string _themeStyles = "";

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

		var themeMode = DesignThemeModes.Dark;
		var cacheKey = ConfigurationService.CacheKey;
		if (TfUserState.Value.CurrentUser is not null && TfUserState.Value.CurrentUser.Settings is not null)
		{
			themeMode = TfUserState.Value.CurrentUser.Settings.ThemeMode;
		}
		if (themeMode == DesignThemeModes.System)
		{
			var browserModeIsDark = await _getIsDarkModeFromBrowser();
			themeMode = browserModeIsDark ? DesignThemeModes.Dark : DesignThemeModes.Light;
		}

		var sb = new StringBuilder();
		//Dark
		if (themeMode == DesignThemeModes.Dark)
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
