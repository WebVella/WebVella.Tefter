namespace WebVella.Tefter.UI.Components;

public partial class TucPageTopbar : TfBaseComponent, IAsyncDisposable
{
	private DotNetObjectReference<TucPageTopbar> _objectRef = null!;
	private bool _userMenuVisible = false;

	public async ValueTask DisposeAsync()
	{
		try
		{
			await JSRuntime.InvokeAsync<object>("Tefter.removeThemeSwitchListener", ComponentId.ToString());
		}
		catch
		{
			//In rare ocasions the item is disposed after the JSRuntime is no longer avaible
		}
		_objectRef.Dispose();
	}

	protected override async Task OnInitializedAsync()
	{
		_objectRef = DotNetObjectReference.Create(this);
		try
		{
			await JSRuntime.InvokeAsync<object>(
			"Tefter.addThemeSwitchListener", _objectRef, ComponentId.ToString(), "OnThemeSwitchHandler");
		}
		catch
		{
			//In rare ocasions the item is disposed after the JSRuntime is no longer avaible
		}
	}



	private async Task _visualPreferencesHandler()
	{
		var dialog = await DialogService.ShowDialogAsync<TucUserVisualPreferencesDialog>(
			TfAuthLayout.GetState().User,
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				TrapFocus = false
			});
		_ = await dialog.Result;
	}


	private async Task _setUrlAsStartup()
	{
		var uri = new Uri(Navigator.Uri);
		try
		{
			_ = await TfService.SetStartUpUrl(
				userId: TfAuthLayout.GetState().User.Id,
				url: uri.PathAndQuery
			);
			ToastService.ShowSuccess(LOC("Startup URL was successfully changed!"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}

	private async Task _resetUrlAsStartup()
	{
		try
		{
			_ = await TfService.SetStartUpUrl(
				userId: TfAuthLayout.GetState().User.Id,
				url: null
			);
			ToastService.ShowSuccess(LOC("Startup URL was successfully changed!"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}


	private bool _isSetAsStartupUri()
	{
		var uri = new Uri(Navigator.Uri);
		return uri.PathAndQuery == TfAuthLayout.GetState().User.Settings.StartUpUrl;
	}

	private async Task _logout()
	{
		await TfService.LogoutAsync(JSRuntime);
		Navigator.NavigateTo(TfConstants.LoginPageUrl, true);
	}

	[JSInvokable("OnThemeSwitchHandler")]
	public async Task OnGlobalSearchHandler()
	{
		try
		{
			DesignThemeModes newMode = DesignThemeModes.Light;
			if (TfAuthLayout.GetState().User.Settings.ThemeMode == DesignThemeModes.Light)
				newMode = DesignThemeModes.Dark;

			_ = await TfService.SetUserTheme(
				userId: TfAuthLayout.GetState().User.Id,
				themeMode: newMode
			);
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}
}