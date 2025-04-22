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
		sb.AppendLine("<style>");

		//Root
		sb.AppendLine(":root {");
		sb.AppendLine("--tf-layout-header-height: 48px;");
		sb.AppendLine("--tf-layout-wrapper-height: calc(100vh - var(--tf-layout-header-height));");
		sb.AppendLine("--tf-layout-wrapper-aside-width: 60px;");
		sb.AppendLine("--tf-layout-wrapper-body-width: calc(100vw - var(--tf-layout-wrapper-aside-width));");
		sb.AppendLine("--tf-layout-wrapper-body-border-radius: 8px;");
		sb.AppendLine("--tf-layout-wrapper-body-aside-width: 240px;");
		sb.AppendLine("--tf-layout-wrapper-body-aside-toolbar-height: 40px;");
		sb.AppendLine("--tf-content-padding: 10px;");
		sb.AppendLine("--tf-header-height: 48px;");
		sb.AppendLine("--tf-content-border-radius: 8px;");
		sb.AppendLine("--tf-content-border-top-width: 3px;");
		sb.AppendLine("--tf-content-margin-bottom: 5px;");
		sb.AppendLine("--tf-content-header-height: 50px;");
		sb.AppendLine("--tf-content-toolbar-height: 60px;");
		sb.AppendLine("--tf-sidebar-toolbar-height: 40px;");
		sb.AppendLine("--tf-sidebar-toolbar-margin-top-bottom: 10px;");
		sb.AppendLine("--tf-body-content-padding: 10px;");
		sb.AppendLine("--tf-grid-padding-top: calc(var(--design-unit) + var(--focus-stroke-width) - var(--stroke-width));");
		sb.AppendLine("--tf-grid-padding-side: calc(((var(--design-unit) * 3) + var(--focus-stroke-width) - var(--stroke-width)) * 1);");
		sb.AppendLine("--tf-grid-negative-margin-top: calc((var(--design-unit) + var(--focus-stroke-width) - var(--stroke-width)) * -1);");
		sb.AppendLine("--tf-grid-negative-margin-side: calc(((var(--design-unit) * 3) + var(--focus-stroke-width) - var(--stroke-width)) * -1);");
		sb.AppendLine("}");
		sb.AppendLine("</style>");
		//Colors
				sb.AppendLine("<style>");
		sb.AppendLine(":root {");
		foreach (var item in Enum.GetValues<TfColor>())
		{
			sb.AppendLine($"{item.ToName()}: {item.ToColorString()};");
		}
		sb.AppendLine("}");
		sb.AppendLine("</style>");

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
