namespace WebVella.Tefter.UI.Components;
public partial class TucAdminUserDetailsAsideContent : TfBaseComponent, IDisposable
{
	[Inject] public ITfUserUIService TfUserUIService { get; set; } = default!;
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;

	private bool _isLoading = true;
	private int _stringLimit = 30;
	private string? _search = String.Empty;
	private List<TfMenuItem> _items = new();
	public void Dispose()
	{
		TfUserUIService.UserCreated -= On_UserCreated;
		TfUserUIService.UserUpdated -= On_UserUpdated;
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfUserUIService.UserCreated += On_UserCreated;
		TfUserUIService.UserUpdated += On_UserUpdated;
		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
	}


	private async void On_UserCreated(object? caller, TfUser user)
	{
		await _init();
	}

	private async void On_UserUpdated(object? caller, TfUser user)
	{
		await _init();
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}

	private async Task _init(TfNavigationState? navState = null)
	{
		if (navState is null)
			navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);

		try
		{
			_search = navState.SearchAside;
			var users = TfUserUIService.GetUsers(_search).ToList();
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