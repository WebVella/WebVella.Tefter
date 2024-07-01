namespace WebVella.Tefter.Web.Components.AppInit;
public partial class TfAppInit : TfBaseComponent
{
	[Inject] private AppStartUseCase UC { get; set; }
	[Parameter] public RenderFragment ChildContent { get; set; }

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
			Navigator.LocationChanged -= Navigator_LocationChanged;
		}

		await base.DisposeAsyncCore(disposing);
	}
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			await UC.OnAfterRenderAsync();
			if(UC.User is null) return;
			//Subscribe for state set results
			//so we can know when all states are inited
			ActionSubscriber.SubscribeToAction<SetUserActionResult>(this, initUserStateResult);
			ActionSubscriber.SubscribeToAction<SetCultureActionResult>(this, initCultureStateResult);
			ActionSubscriber.SubscribeToAction<SetThemeActionResult>(this, initThemeStateResult);
			ActionSubscriber.SubscribeToAction<SetSidebarActionResult>(this, initSidebarStateResult);

			//Setup states
			Dispatcher.Dispatch(new InitUserStateAction(
				user:UC.User
			));
			Dispatcher.Dispatch(new InitCultureStateAction(
				culture: UC.CultureOption
			));
			Dispatcher.Dispatch(new InitThemeStateAction(
				themeMode: UC.User.Settings?.ThemeMode ?? TfConstants.DefaultThemeMode,
				themeColor: UC.User.Settings?.ThemeColor ?? TfConstants.DefaultThemeColor
			));
			Dispatcher.Dispatch(new InitScreenStateAction(
				sidebarExpanded: UC.User.Settings?.IsSidebarOpen ?? true
			));

			//For the logout fix
			Navigator.LocationChanged += Navigator_LocationChanged;

			//_isLoading = false;
			//await InvokeAsync(StateHasChanged);
		}
	}

	/// <summary>
	/// Processes the user state init action result
	/// </summary>
	/// <param name="action"></param>
	private void initUserStateResult(SetUserActionResult action)
	{
		UC.UserStateInited = true;
		CheckAllInited();
	}

	/// <summary>
	/// Processes the culture state init action result
	/// </summary>
	/// <param name="action"></param>
	private void initCultureStateResult(SetCultureActionResult action)
	{
		UC.CultureStateInited = true;
		CheckAllInited();
	}

	/// <summary>
	/// Processes the theme state init action result
	/// </summary>
	/// <param name="action"></param>
	private void initThemeStateResult(SetThemeActionResult action)
	{
		if (!UC.IsLoading)
		{
			Navigator.ReloadCurrentUrl();
			return;
		}
		UC.ThemeStateInited = true;
		CheckAllInited();

	}

	/// <summary>
	/// Processes the sidebar state init action result
	/// </summary>
	/// <param name="action"></param>
	private void initSidebarStateResult(SetSidebarActionResult action)
	{
		UC.SidebarStateInited = true;
		CheckAllInited();
	}

	/// <summary>
	/// If all inited, removes the loading state
	/// </summary>
	private void CheckAllInited()
	{
		if (UC.AllInited)
		{
			UC.IsLoading = false;
			StateHasChanged();
		}
	}

	/// <summary>
	/// Fixing a problem when loging out from one tab leaves the user logged on the others
	/// Space data
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
		InvokeAsync(async () =>
		{
			await UC.OnLocationChange();
		});
	}

}
