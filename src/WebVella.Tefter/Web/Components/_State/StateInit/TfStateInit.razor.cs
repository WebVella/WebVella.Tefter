namespace WebVella.Tefter.Web.Components;
public partial class TfStateInit : TfBaseComponent
{
	[Inject] private StateInitUseCase UC { get; set; }
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
			var state = await UC.InitState();
			if (state.CurrentUser is null)
			{
				Navigator.NavigateTo(TfConstants.LoginPageUrl, true);
				return;
			}
			else if (state.Culture is null)
			{
				Navigator.ReloadCurrentUrl();
				return;
			}
			state = state with
			{
				ThemeMode = state.CurrentUser.Settings?.ThemeMode ?? TfConstants.DefaultThemeMode,
				ThemeColor = state.CurrentUser.Settings?.ThemeColor ?? TfConstants.DefaultThemeColor,
				SidebarExpanded = state.CurrentUser.Settings?.IsSidebarOpen ?? true
			};

			//Setup states
			Dispatcher.Dispatch(new InitStateAction(
				component: this,
				state: state
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
