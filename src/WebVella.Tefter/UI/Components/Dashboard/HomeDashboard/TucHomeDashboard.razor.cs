
namespace WebVella.Tefter.UI.Components;

public partial class TucHomeDashboard : TfBaseComponent
{
	[Inject] protected ITfConfigurationService TfConfigurationService { get; set; } = null!;
	protected override async Task OnInitializedAsync()
	{
		_checkRedirect(Navigator.Uri);
	}
	private void _checkRedirect(string url)
	{
		var uri = new Uri(url);
		var _currentUser = TfAuthLayout.GetState().User;
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
				var spacePages = TfService.GetSpacePages(space.Id);
				if(spacePages.Count == 0) continue;
				Navigator.NavigateTo(string.Format(TfConstants.SpacePagePageUrl, space.Id, spacePages[0].Id),
					true);				
				
				return;
			}
		}		
	}
}