namespace WebVella.Tefter.Web.Components;

[LocalizationResource("WebVella.Tefter.Web.Components.General.UserNavigation.TfUserNavigation", "WebVella.Tefter")]
public partial class TfUserNavigation
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] private UserStateUseCase UC { get; set; }

	private bool _visible = false;
	private bool _helpVisible = false;
	private bool _isAdmin = false;
	private bool _auxLoaded = false;

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			Navigator.LocationChanged -= Navigator_LocationChanged;
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();
		initAdmin(null);
		Navigator.LocationChanged += Navigator_LocationChanged;
	}
	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
		initAdmin(e.Location);
		StateHasChanged();
	}

	private void initAdmin(string location)
	{
		Uri uri = null;
		if (string.IsNullOrEmpty(location))
		{
			uri = new Uri(Navigator.Uri);
		}
		else
		{
			uri = new Uri(location);
		}
		_isAdmin = uri.LocalPath.StartsWith("/admin");
	}

	private void _onClick()
	{
		_visible = !_visible;
	}

	private async Task _setTheme()
	{
		var themeSettings = new TucThemeSettings { ThemeMode = TfUserState.Value.ThemeMode, ThemeColor = TfUserState.Value.ThemeColor };
		var dialog = await DialogService.ShowDialogAsync<TfThemeSetDialog>(themeSettings, new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var response = (TucThemeSettings)result.Data;
			try
			{
				var resultSrv = await UC.SetUserTheme(
							userId: TfUserState.Value.CurrentUser.Id,
							themeMode: response.ThemeMode,
							themeColor: response.ThemeColor
						);
				ProcessServiceResponse(resultSrv);
				if (resultSrv.IsSuccess)
				{
					ToastService.ShowSuccess(LOC("The theme configurations were successfully changed!"));
					Dispatcher.Dispatch(new SetUserStateAction(
						component:this,
						state: TfUserState.Value with { CurrentUser = resultSrv.Value}));
				}
			}
			catch (Exception ex)
			{
				ProcessException(ex);
			}


		}
	}

	private async Task _logout()
	{
		await IdentityManager.LogoutAsync(JSRuntime);
		Navigator.NavigateTo(TfConstants.LoginPageUrl, true);

	}

}