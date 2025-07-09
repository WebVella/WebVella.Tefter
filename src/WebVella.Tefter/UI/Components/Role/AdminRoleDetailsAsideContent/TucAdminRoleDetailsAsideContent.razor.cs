namespace WebVella.Tefter.UI.Components;
public partial class TucAdminRoleDetailsAsideContent : TfBaseComponent, IDisposable
{
	[Inject] public ITfUserUIService TfUserUIService { get; set; } = default!;
	[Inject] public ITfSpaceUIService TfSpaceUIService { get; set; } = default!;

	private bool _isLoading = true;
	private int _stringLimit = 30;
	private string? _search = String.Empty;
	private List<TfMenuItem> _items = new();
	public void Dispose()
	{
		TfUserUIService.RoleCreated -= On_RoleCreated;
		TfUserUIService.RoleUpdated -= On_RoleUpdated;
		TfUserUIService.RoleDeleted -= On_RoleDeleted;
		TfSpaceUIService.NavigationDataChanged -= On_NavigationDataChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfUserUIService.RoleCreated += On_RoleCreated;
		TfUserUIService.RoleUpdated += On_RoleUpdated;
		TfUserUIService.RoleDeleted += On_RoleDeleted;
		TfSpaceUIService.NavigationDataChanged += On_NavigationDataChanged;
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
			var roles = TfUserUIService.GetRoles(_search).ToList();

			_items = new();
			foreach (var role in roles)
			{
				_items.Add(new TfMenuItem
				{
					Url = string.Format(TfConstants.AdminRoleDetailsPageUrl, role.Id),
					Description = role.IsSystem ? "system created" : "user created",
					Text = TfConverters.StringOverflow(role.Name, _stringLimit),
					Selected = navData.State.RoleId == role.Id
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