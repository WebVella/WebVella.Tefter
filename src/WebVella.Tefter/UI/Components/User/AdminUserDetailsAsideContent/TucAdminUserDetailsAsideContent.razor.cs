namespace WebVella.Tefter.UI.Components;
public partial class TucAdminUserDetailsAsideContent : TfBaseComponent, IDisposable
{
	private bool _isLoading = true;
	private int _stringLimit = 30;
	private string? _search = String.Empty;
	private List<TfMenuItem> _items = new();
	public void Dispose()
	{
		TfUIService.UserCreated -= On_UserCreated;
		TfUIService.UserUpdated -= On_UserUpdated;
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init(Navigator.GetRouteState());
		TfUIService.UserCreated += On_UserCreated;
		TfUIService.UserUpdated += On_UserUpdated;
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
	}


	private async void On_UserCreated(object? caller, TfUser user)
	{
		await _init(Navigator.GetRouteState());
	}

	private async void On_UserUpdated(object? caller, TfUser user)
	{
		await _init(Navigator.GetRouteState());
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}

	private async Task _init(TfNavigationState navState)
	{
		try
		{
			_search = navState.SearchAside;
			var users = TfUIService.GetUsers(_search).ToList();
			_items = new();
			foreach (var user in users)
			{
				_items.Add(new TfMenuItem
				{
					Url = string.Format(TfConstants.AdminUserDetailsPageUrl, user.Id),
					Description = user.Email,
					Text = TfConverters.StringOverflow(user.Names, _stringLimit),
					Selected = navState.UserId == user.Id
				});
			}
		}
		finally
		{
			_isLoading = false;
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}
}