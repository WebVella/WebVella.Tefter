namespace WebVella.Tefter.Web.Components;
public partial class TfAppInit : TfBaseComponent
{
	[Inject] private AppStartUseCase UC { get; set; }
	[Parameter] public RenderFragment ChildContent { get; set; }

	//protected override async ValueTask DisposeAsyncCore(bool disposing)
	//{
	//	if (disposing)
	//	{
	//		Navigator.LocationChanged -= Navigator_LocationChanged;
	//	}

	//	await base.DisposeAsyncCore(disposing);
	//}
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			var initResult = await UC.InitState();
			if(initResult is null 
				|| initResult.User is null 
				|| initResult.CultureOption is null) 
				return;

			//Setup states
			Dispatcher.Dispatch(new InitStateAction(
				component: this,
				user: initResult.User,
				userSpaces: initResult.UserSpaces,
				culture: initResult.CultureOption,
				themeMode: initResult.User.Settings?.ThemeMode ?? TfConstants.DefaultThemeMode,
				themeColor: initResult.User.Settings?.ThemeColor ?? TfConstants.DefaultThemeColor,
				sidebarExpanded: initResult.User.Settings?.IsSidebarOpen ?? true
			));
			//For the logout fix - Not needed anymore
			//Navigator.LocationChanged += Navigator_LocationChanged;
			UC.IsBusy = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	/// <summary>
	/// Fixing a problem when loging out from one tab leaves the user logged on the others
	/// Space data
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	//private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	//{
	//	InvokeAsync(async () =>
	//	{
	//		//await UC.OnLocationChange();
	//	});
	//}

}
