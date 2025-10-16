
namespace WebVella.Tefter.UI.Components;

public partial class TucHomeNoSpace : TfBaseComponent
{
	[Inject] protected ITfConfigurationService TfConfigurationService { get; set; } = null!;

	private TfUser _currentUser = null!;
	protected override async Task OnInitializedAsync()
	{
		_checkRedirect(Navigator.Uri);
	}
	private void _checkRedirect(string url)
	{
		var uri = new Uri(url);
		_currentUser = TfAuthLayout.GetState().User;
		if (uri.LocalPath == "/")
		{
			var queryDictionary = HttpUtility.ParseQueryString(uri.Query);
			Uri? startupUri = null;
			if (!String.IsNullOrWhiteSpace(_currentUser.Settings.StartUpUrl))
			{
				if (_currentUser.Settings.StartUpUrl.StartsWith("http:"))
					startupUri = new Uri(_currentUser.Settings.StartUpUrl);
				else
					startupUri = new Uri(TfConfigurationService.BaseUrl + _currentUser.Settings.StartUpUrl);
			}

			if (startupUri != null && uri.LocalPath != startupUri.LocalPath
			                       && queryDictionary[TfConstants.NoDefaultRedirectQueryName] is null)
			{
				Navigator.NavigateTo(_currentUser.Settings.StartUpUrl!, true);
				return;
			}

			foreach (var space in TfService.GetSpacesListForUser(_currentUser.Id))
			{
				Navigator.NavigateTo(string.Format(TfConstants.SpacePageUrl, space.Id),
					true);				
				
				return;
			}
		}		
	}
	
	private async Task _addSpace()
	{
		var dialog = await DialogService.ShowDialogAsync<TucSpaceManageDialog>(
			new TfSpace(),
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var space = (TfSpace)result.Data;
			Navigator.NavigateTo(String.Format(TfConstants.SpacePageUrl, space.Id));
		}
	}		
}