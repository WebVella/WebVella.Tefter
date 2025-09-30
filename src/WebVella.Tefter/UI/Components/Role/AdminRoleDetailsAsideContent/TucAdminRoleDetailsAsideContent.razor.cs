namespace WebVella.Tefter.UI.Components;
public partial class TucAdminRoleDetailsAsideContent : TfBaseComponent, IDisposable
{
	private bool _isLoading = true;
	private int _stringLimit = 30;
	private string? _search = String.Empty;
	private List<TfMenuItem> _items = new();
	public void Dispose()
	{
		TfUIService.RoleCreated -= On_RoleCreated;
		TfUIService.RoleUpdated -= On_RoleUpdated;
		TfUIService.RoleDeleted -= On_RoleDeleted;
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfUIService.RoleCreated += On_RoleCreated;
		TfUIService.RoleUpdated += On_RoleUpdated;
		TfUIService.RoleDeleted += On_RoleDeleted;
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_RoleCreated(object? caller, TfRole user)
	{
		await _init();
	}

	private async void On_RoleUpdated(object? caller, TfRole user)
	{
		await _init();
	}

	private async void On_RoleDeleted(object? caller, TfRole user)
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
			navState = TfAuthLayout.NavigationState;

		try
		{
			_search = navState.SearchAside;
			var roles = TfUIService.GetRoles(_search).ToList();

			_items = new();
			foreach (var role in roles)
			{
				_items.Add(new TfMenuItem
				{
					Url = string.Format(TfConstants.AdminRoleDetailsPageUrl, role.Id),
					Description = role.IsSystem ? "system created" : "user created",
					Text = TfConverters.StringOverflow(role.Name, _stringLimit),
					Selected = navState.RoleId == role.Id
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