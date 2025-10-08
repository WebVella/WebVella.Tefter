namespace WebVella.Tefter.UI.Components;
public partial class TucAdminUsersPageContent :TfBaseComponent, IDisposable
{
	private bool _isLoading = false;
	private List<TfUser>? _items = null;
	
	public void Dispose()
	{
		TfEventProvider.UserCreatedGlobalEvent -= On_UserChanged;
		TfEventProvider.UserUpdatedGlobalEvent -= On_UserChanged;
		TfState.NavigationStateChangedEvent -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init(TfState.NavigationState);
		TfEventProvider.UserCreatedGlobalEvent += On_UserChanged;
		TfEventProvider.UserUpdatedGlobalEvent += On_UserChanged;
		TfState.NavigationStateChangedEvent += On_NavigationStateChanged;
	}
	
	private async Task On_UserChanged(object args)
	{
		await InvokeAsync(async () =>
		{
			await _init(TfState.NavigationState);
		});
	}	
	
	private async Task On_NavigationStateChanged(TfNavigationState args)
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