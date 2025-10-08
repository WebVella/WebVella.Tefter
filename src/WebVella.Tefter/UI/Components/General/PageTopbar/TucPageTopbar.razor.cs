namespace WebVella.Tefter.UI.Components;

public partial class TucPageTopbar : TfBaseComponent, IDisposable
{
	private TfBookmark? _activeSavedUrl = null!;
	private TucPageLinkSaveSelector _saveSelector = null!;
	private bool _userMenuVisible = false;

	public void Dispose()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
	}

	protected override void OnInitialized()
	{
		_init();
		Navigator.LocationChanged += On_NavigationStateChanged;
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		_init();
		StateHasChanged();
	}

	private void _init()
	{
		_activeSavedUrl = TfAuthLayout.GetState().UserSaves.FirstOrDefault(x => x.Id == TfAuthLayout.GetState().NavigationState.ActiveSaveId);
	}

	private async Task _onSaveLinkClick()
	{
		if (TfAuthLayout.GetState().SpacePage is null)
			return;
		if (_activeSavedUrl is not null)
		{
			await _saveSelector.ToggleSelector();
			return;
		}

		try
		{
			var submit = new TfBookmark
			{
				Id = Guid.NewGuid(),
				SpacePageId = TfAuthLayout.GetState().SpacePage.Id,
				UserId = TfAuthLayout.GetState().User.Id,
				CreatedOn = DateTime.Now,
				Description = String.Empty, //initially nothing is added for convenience
				Name = TfAuthLayout.GetState().SpacePage.Name + " " + DateTime.Now.ToString("dd-MM-yyyy HH:mm"),
				Url = new Uri(Navigator.Uri).PathAndQuery
			};
			TfService.CreateBookmark(submit);

			ToastService.ShowSuccess(LOC("URL is now saved"));
			await Navigator.ApplyChangeToUrlQuery(TfConstants.ActiveSaveQueryName, submit.Id);
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			await InvokeAsync(StateHasChanged);
		}
	}

	async Task _setUrlAsStartup()
	{
		var uri = new Uri(Navigator.Uri);
		try
		{
			var user = await TfService.SetStartUpUrl(
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

	private async Task _logout()
	{
		await TfService.LogoutAsync(JSRuntime);
		Navigator.NavigateTo(TfConstants.LoginPageUrl, true);
	}
}