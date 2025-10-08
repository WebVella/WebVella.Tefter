namespace WebVella.Tefter.UI.Components;

public partial class TucAdminUsersPageContent : TfBaseComponent, IDisposable
{
	private bool _isLoading = false;
	private List<TfUser> _items = new();

	public void Dispose()
	{
		TfEventProvider.UserCreatedGlobalEvent -= On_UserChanged;
		TfEventProvider.UserUpdatedGlobalEvent -= On_UserChanged;
		Navigator.LocationChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		TfEventProvider.UserCreatedGlobalEvent += On_UserChanged;
		TfEventProvider.UserUpdatedGlobalEvent += On_UserChanged;
		Navigator.LocationChanged += On_NavigationStateChanged;
	}

	private async Task On_UserChanged(object args)
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
			{
				await _init(TfAuthLayout.GetState().NavigationState);
			}
		});
	}

	private async Task _init(TfNavigationState navState)
	{
		try
		{
			_items = TfService.GetUsers(navState.Search).ToList();
		}
		finally
		{
			_isLoading = false;
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}
}