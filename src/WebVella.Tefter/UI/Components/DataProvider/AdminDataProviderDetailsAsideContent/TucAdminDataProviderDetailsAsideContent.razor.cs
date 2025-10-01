namespace WebVella.Tefter.UI.Components;
public partial class TucAdminDataProviderDetailsAsideContent : TfBaseComponent, IDisposable
{
	private bool _isLoading = true;
	private int _stringLimit = 30;
	private string? _search = String.Empty;
	private List<TfMenuItem> _items = new();
	public void Dispose()
	{
		TfUIService.DataProviderCreated -= On_DataProviderCreated;
		TfUIService.DataProviderUpdated -= On_DataProviderUpdated;
		TfUIService.DataProviderDeleted -= On_DataProviderDeleted;
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await _init(Navigator.GetRouteState());
		TfUIService.DataProviderCreated += On_DataProviderCreated;
		TfUIService.DataProviderUpdated += On_DataProviderUpdated;
		TfUIService.DataProviderDeleted += On_DataProviderDeleted;
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_DataProviderCreated(object? caller, TfDataProvider user)
	{
		await _init(Navigator.GetRouteState());
	}

	private async void On_DataProviderUpdated(object? caller, TfDataProvider user)
	{
		await _init(Navigator.GetRouteState());
	}

	private async void On_DataProviderDeleted(object? caller, TfDataProvider user)
	{
		await _init(Navigator.GetRouteState());
	}


	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}

	private async Task _init(TfNavigationState navState)
	{
		try
		{
			_search = navState.SearchAside;
			var items = TfUIService.GetDataProviders(_search).ToList();
			_items = new();
			foreach (var item in items)
			{
				_items.Add(new TfMenuItem
				{
					Url = string.Format(TfConstants.AdminDataProviderDetailsPageUrl, item.Id),
					Description = item.ProviderType.AddonName,
					Text = TfConverters.StringOverflow(item.Name, _stringLimit),
					Selected = navState.DataProviderId == item.Id
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