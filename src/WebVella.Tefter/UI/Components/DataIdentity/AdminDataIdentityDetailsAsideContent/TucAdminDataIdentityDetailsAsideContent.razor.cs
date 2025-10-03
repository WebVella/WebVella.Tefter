namespace WebVella.Tefter.UI.Components;
public partial class TucAdminDataIdentityDetailsAsideContent : TfBaseComponent, IDisposable
{
	private bool _isLoading = true;
	private int _stringLimit = 30;
	private string? _search = String.Empty;
	private List<TfMenuItem> _items = new();
	public void Dispose()
	{
		TfEventProvider.DataIdentityCreatedEvent -= On_DataIdentityChanged;
		TfEventProvider.DataIdentityUpdatedEvent -= On_DataIdentityChanged;
		TfEventProvider.DataIdentityDeletedEvent -= On_DataIdentityChanged;
		TfEventProvider.NavigationStateChangedEvent -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await _init(TfAuthLayout.NavigationState);
		TfEventProvider.DataIdentityCreatedEvent += On_DataIdentityChanged;
		TfEventProvider.DataIdentityUpdatedEvent += On_DataIdentityChanged;
		TfEventProvider.DataIdentityDeletedEvent += On_DataIdentityChanged;
		TfEventProvider.NavigationStateChangedEvent += On_NavigationStateChanged;
	}

	private async void On_DataIdentityChanged(object args)
	{
		await _init(TfAuthLayout.NavigationState);
	}


	private async void On_NavigationStateChanged(TfNavigationStateChangedEvent args)
	{
		if (args.IsUserApplicable(TfAuthLayout.CurrentUser) && UriInitialized != args.Payload.Uri)
			await _init(args.Payload);
	}

	private async Task _init(TfNavigationState navState)
	{
		try
		{
			_search = navState.SearchAside;
			var roles = TfService.GetDataIdentities(_search).ToList();
			_items = new();
			foreach (var role in roles)
			{
				_items.Add(new TfMenuItem
				{
					Url = string.Format(TfConstants.AdminDataIdentityDetailsPageUrl, role.DataIdentity),
					Description = role.IsSystem ? "system created" : "user created",
					Text = TfConverters.StringOverflow(role.DataIdentity, _stringLimit),
					Selected = navState.DataIdentityId == role.DataIdentity
				});
			}
		}
		finally
		{
			UriInitialized = navState.Uri;
			_isLoading = false;
			await InvokeAsync(StateHasChanged);
		}
	}
}