namespace WebVella.Tefter.UI.Components;
public partial class TucAdminUserDetailsAsideContent : TfBaseComponent, IDisposable
{
	[Inject] protected TfGlobalEventProvider TfEventProvider { get; set; } = null!;
	private bool _isLoading = true;
	private int _stringLimit = 30;
	private string? _search = String.Empty;
	private List<TfMenuItem> _items = new();
	public void Dispose()
	{
		TfEventProvider?.Dispose();
		Navigator.LocationChanged -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		TfEventProvider.UserCreatedEvent += On_UserCreated;
		TfEventProvider.UserUpdatedEvent += On_UserUpdated;
		Navigator.LocationChanged += On_NavigationStateChanged;
	}


	private async Task On_UserCreated(TfUserCreatedEvent args)
	{
		await InvokeAsync(async () =>
		{
			await _init(TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task On_UserUpdated(TfUserUpdatedEvent args)
	{
		await InvokeAsync(async () =>
		{
			await _init(TfAuthLayout.GetState().NavigationState);
		});
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
				await _init(TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task _init(TfNavigationState navState)
	{
		try
		{
			_search = navState.SearchAside;
			var users = TfService.GetUsers(_search).ToList();
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