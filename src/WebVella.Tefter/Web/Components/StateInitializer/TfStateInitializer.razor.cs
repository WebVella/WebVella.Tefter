namespace WebVella.Tefter.Web.Components.StateInitializer;
public partial class TfStateInitializer : TfBaseComponent
{
	[Inject] private AppStartUseCase UseCase { get; set; }
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
			await UseCase.InitializeAsync();

			//Subscribe for state set results
			//so we can know when all states are inited
			ActionSubscriber.SubscribeToAction<SetUserActionResult>(this, initUserStateResult);
			ActionSubscriber.SubscribeToAction<SetCultureActionResult>(this, initCultureStateResult);
			ActionSubscriber.SubscribeToAction<SetThemeActionResult>(this, initThemeStateResult);
			ActionSubscriber.SubscribeToAction<SetSidebarActionResult>(this, initSidebarStateResult);

			//Setup states
			Dispatcher.Dispatch(new InitUserStateAction(
				user:UseCase.User
			));
			Dispatcher.Dispatch(new InitCultureStateAction(
				culture: UseCase.CultureOption
			));
			Dispatcher.Dispatch(new InitThemeStateAction(
				themeMode: UseCase.User.Settings?.ThemeMode ?? TfConstants.DefaultThemeMode,
				themeColor: UseCase.User.Settings?.ThemeColor ?? TfConstants.DefaultThemeColor
			));
			Dispatcher.Dispatch(new InitScreenStateAction(
				sidebarExpanded: UseCase.User.Settings?.IsSidebarOpen ?? true
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
		UseCase.UserStateInited = true;
		CheckAllInited();
	}

	/// <summary>
	/// Processes the culture state init action result
	/// </summary>
	/// <param name="action"></param>
	private void initCultureStateResult(SetCultureActionResult action)
	{
		UseCase.CultureStateInited = true;
		CheckAllInited();
	}

	/// <summary>
	/// Processes the theme state init action result
	/// </summary>
	/// <param name="action"></param>
	private void initThemeStateResult(SetThemeActionResult action)
	{
		if (!UseCase.IsLoading)
		{
			Navigator.ReloadCurrentUrl();
			return;
		}
		UseCase.ThemeStateInited = true;
		CheckAllInited();

	}

	/// <summary>
	/// Processes the sidebar state init action result
	/// </summary>
	/// <param name="action"></param>
	private void initSidebarStateResult(SetSidebarActionResult action)
	{
		UseCase.SidebarStateInited = true;
		CheckAllInited();
	}

	/// <summary>
	/// If all inited, removes the loading state
	/// </summary>
	private void CheckAllInited()
	{
		if (UseCase.AllInited)
		{
			UseCase.IsLoading = false;
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
			await UseCase.OnLocationChange();
		});
	}

}
