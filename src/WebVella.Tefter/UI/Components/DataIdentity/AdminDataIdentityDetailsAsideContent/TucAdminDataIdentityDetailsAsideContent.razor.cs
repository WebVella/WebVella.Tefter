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
		Navigator.LocationChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await _init(TfAuthLayout.GetState().NavigationState);
		TfEventProvider.DataIdentityCreatedEvent += On_DataIdentityChanged;
		TfEventProvider.DataIdentityUpdatedEvent += On_DataIdentityChanged;
		TfEventProvider.DataIdentityDeletedEvent += On_DataIdentityChanged;
		Navigator.LocationChanged += On_NavigationStateChanged;
	}

	private async Task On_DataIdentityChanged(object args)
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