namespace WebVella.Tefter.Web.Components;
public partial class TfUserNavigation
{
	[Inject] protected IState<UserState> UserState { get; set; }
	[Inject] protected IState<SessionState> SessionState { get; set; }

	private bool _visible = false;
	private void _onClick()
	{
		_visible = !_visible;
	}

	private async Task _setTheme()
	{
		var themeSettings = new ThemeSettings { ThemeMode = SessionState.Value.ThemeMode, ThemeColor = SessionState.Value.ThemeColor };
		var dialog = await DialogService.ShowDialogAsync<TfThemeSetDialog>(themeSettings, new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var response = (ThemeSettings)result.Data;
			Dispatcher.Dispatch(new SetThemeAction(response.ThemeMode, response.ThemeColor));

		}
	}
	private void _editProfile()
	{
		ToastService.ShowToast(ToastIntent.Warning, "Will open edit profile modal");
	}
	private void _logout()
	{
		Dispatcher.Dispatch(new LogoutUserAction());

	}

	private void _alertsClick()
	{
		ToastService.ShowToast(ToastIntent.Warning, "Navigates to usr alerts");
	}

	private void _adminClick()
	{
		ToastService.ShowToast(ToastIntent.Warning, "Navigates to Administration panel");
	}
}