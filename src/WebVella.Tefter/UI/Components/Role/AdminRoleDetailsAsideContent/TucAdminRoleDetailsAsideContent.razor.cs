namespace WebVella.Tefter.UI.Components;
public partial class TucAdminRoleDetailsAsideContent : TfBaseComponent, IDisposable
{
	private bool _isLoading = true;
	private int _stringLimit = 30;
	private string? _search = String.Empty;
	private List<TfMenuItem> _items = new();
	public void Dispose()
	{
		TfEventProvider.RoleCreatedEvent -= On_RoleChanged;
		TfEventProvider.RoleUpdatedEvent -= On_RoleChanged;
		TfEventProvider.RoleDeletedEvent -= On_RoleChanged;
		TfAuthLayout.NavigationStateChangedEvent -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.NavigationState);
		TfEventProvider.RoleCreatedEvent += On_RoleChanged;
		TfEventProvider.RoleUpdatedEvent += On_RoleChanged;
		TfEventProvider.RoleDeletedEvent += On_RoleChanged;
		TfAuthLayout.NavigationStateChangedEvent += On_NavigationStateChanged;
	}

	private async void On_RoleChanged(object args)
	{
		await InvokeAsync(async () =>
		{
			await _init(TfAuthLayout.NavigationState);
		});
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		await InvokeAsync(async () =>
		{
			if (UriInitialized != args.Uri)
				await _init(args);
		});
	}

	private async Task _init(TfNavigationState navState)
	{
		try
		{
			_search = navState.SearchAside;
			var roles = TfService.GetRoles(_search).ToList();

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