namespace WebVella.Tefter.UI.Components;
public partial class TucAdminUserDetailsAsideContent : TfBaseComponent, IDisposable
{
	private bool _isLoading = true;
	private int _stringLimit = 30;
	private string? _search = String.Empty;
	private List<TfMenuItem> _items = new();
	public void Dispose()
	{
		TfEventProvider.UserCreatedGlobalEvent -= On_UserCreated;
		TfEventProvider.UserUpdatedGlobalEvent -= On_UserUpdated;
		TfAuthLayout.NavigationStateChangedEvent -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.NavigationState);
		TfEventProvider.UserCreatedGlobalEvent += On_UserCreated;
		TfEventProvider.UserUpdatedGlobalEvent += On_UserUpdated;
		TfAuthLayout.NavigationStateChangedEvent += On_NavigationStateChanged;
	}


	private async void On_UserCreated(TfUserCreatedEvent args)
	{
		await InvokeAsync(async () =>
		{
			await _init(TfAuthLayout.NavigationState);
		});
	}

	private async void On_UserUpdated(TfUserUpdatedEvent args)
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