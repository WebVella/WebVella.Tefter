namespace WebVella.Tefter.UI.Components;
public partial class TucAdminDataProviderDetailsAsideContent : TfBaseComponent, IDisposable
{
	private bool _isLoading = true;
	private int _stringLimit = 30;
	private string? _search = String.Empty;
	private List<TfMenuItem> _items = new();
	public void Dispose()
	{
		TfEventProvider.DataProviderCreatedEvent -= On_DataProviderChanged;
		TfEventProvider.DataProviderUpdatedEvent -= On_DataProviderChanged;
		TfEventProvider.DataProviderDeletedEvent -= On_DataProviderChanged;
		TfEventProvider.NavigationStateChangedEvent -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await _init(TfAuthLayout.NavigationState);
		TfEventProvider.DataProviderCreatedEvent += On_DataProviderChanged;
		TfEventProvider.DataProviderUpdatedEvent += On_DataProviderChanged;
		TfEventProvider.DataProviderDeletedEvent += On_DataProviderChanged;
		TfEventProvider.NavigationStateChangedEvent += On_NavigationStateChanged;
	}

	private async void On_DataProviderChanged(object  user)
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
			var items = TfService.GetDataProviders(_search).ToList();
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