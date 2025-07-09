namespace WebVella.Tefter.UI.Components;
public partial class TucAdminUserDetailsAsideContent : TfBaseComponent, IDisposable
{
	[Inject] public ITfUserUIService TfUserUIService { get; set; } = default!;
	[Inject] public ITfSpaceUIService TfSpaceUIService { get; set; } = default!;

	private bool _isLoading = true;
	private int _stringLimit = 30;
	private string? _search = String.Empty;
	private List<TfMenuItem> _items = new();
	public void Dispose()
	{
		TfUserUIService.UserCreated -= On_UserCreated;
		TfUserUIService.UserUpdated -= On_UserUpdated;
		TfSpaceUIService.NavigationDataChanged -= On_NavigationDataChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfUserUIService.UserCreated += On_UserCreated;
		TfUserUIService.UserUpdated += On_UserUpdated;
		TfSpaceUIService.NavigationDataChanged += On_NavigationDataChanged;
	}


	private async void On_UserCreated(object? caller, TfUser user)
	{
		await _init();
	}

	private async void On_UserUpdated(object? caller, TfUser user)
	{
		await _init();
	}

	private async void On_NavigationDataChanged(object? caller, TfSpaceNavigationData args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}

	private async Task _init(TfSpaceNavigationData? navData = null)
	{
		if (navData is null)
			navData = await TfSpaceUIService.GetSpaceNavigationData(Navigator);

		try
		{
			_search = navData.State.Search;
			var users = TfUserUIService.GetUsers(_search).ToList();
			_items = new();
			foreach (var user in users)
			{
				_items.Add(new TfMenuItem
				{
					IconColor = user.Settings.ThemeColor,
					Abbriviation = TfConverters.GetUserInitials(user),
					Url = string.Format(TfConstants.AdminUserDetailsPageUrl, user.Id),
					Description = user.Email,
					Text = TfConverters.StringOverflow(user.Names, _stringLimit),
					Selected = navData.State.UserId == user.Id
				});
			}
		}
		finally
		{
			_isLoading = false;
			UriInitialized = navData.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}
}