namespace WebVella.Tefter.UI.Components;
public partial class TucAdminDataProviderDetailsAsideContent : TfBaseComponent, IDisposable
{
	[Inject] public ITfDataProviderUIService TfDataProviderUIService { get; set; } = default!;
	[Inject] public ITfSpaceUIService TfSpaceUIService { get; set; } = default!;

	private bool _isLoading = true;
	private int _stringLimit = 30;
	private string? _search = String.Empty;
	private List<TfMenuItem> _items = new();
	public void Dispose()
	{
		TfDataProviderUIService.DataProviderCreated -= On_DataProviderCreated;
		TfDataProviderUIService.DataProviderUpdated -= On_DataProviderUpdated;
		TfDataProviderUIService.DataProviderDeleted -= On_DataProviderDeleted;
		TfSpaceUIService.NavigationDataChanged -= On_NavigationDataChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await _init();
		TfDataProviderUIService.DataProviderCreated += On_DataProviderCreated;
		TfDataProviderUIService.DataProviderUpdated += On_DataProviderUpdated;
		TfDataProviderUIService.DataProviderDeleted += On_DataProviderDeleted;
		TfSpaceUIService.NavigationDataChanged += On_NavigationDataChanged;
	}

	private async void On_DataProviderCreated(object? caller, TfDataProvider user)
	{
		await _init();
	}

	private async void On_DataProviderUpdated(object? caller, TfDataProvider user)
	{
		await _init();
	}

	private async void On_DataProviderDeleted(object? caller, TfDataProvider user)
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
			var items = TfDataProviderUIService.GetDataProviders(_search).ToList();
			_items = new();
			foreach (var item in items)
			{
				_items.Add(new TfMenuItem
				{
					Url = string.Format(TfConstants.AdminDataProviderDetailsPageUrl, item.Id),
					Description = item.ProviderType.AddonName,
					Text = TfConverters.StringOverflow(item.Name, _stringLimit),
					Selected = navData.State.DataProviderId == item.Id
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