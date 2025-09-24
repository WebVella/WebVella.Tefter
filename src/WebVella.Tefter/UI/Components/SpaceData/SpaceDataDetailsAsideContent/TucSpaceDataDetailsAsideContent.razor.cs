namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceDataDetailsAsideContent : TfBaseComponent, IDisposable
{
	[Inject] public ITfDataProviderUIService TfDataProviderUIService { get; set; } = default!;
	[Inject] public ITfSpaceDataUIService TfSpaceDataUIService { get; set; } = default!;
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;

	private bool _isLoading = true;
	private int _stringLimit = 30;
	private string? _search = String.Empty;
	private List<TfMenuItem> _items = new();
	private TfNavigationState _navState = new();
	public void Dispose()
	{
		TfSpaceDataUIService.SpaceDataCreated -= On_SpaceDataChanged;
		TfSpaceDataUIService.SpaceDataUpdated -= On_SpaceDataChanged;
		TfSpaceDataUIService.SpaceDataDeleted -= On_SpaceDataChanged;
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfSpaceDataUIService.SpaceDataCreated += On_SpaceDataChanged;
		TfSpaceDataUIService.SpaceDataUpdated += On_SpaceDataChanged;
		TfSpaceDataUIService.SpaceDataDeleted += On_SpaceDataChanged;
		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_SpaceDataChanged(object? caller, TfDataSet args)
	{
		await _init();
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}

	private async Task _init(TfNavigationState? navState = null)
	{
		if (navState is null)
			_navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		else
			_navState = navState;

		try
		{
			_items = new();
			if (_navState.SpaceId is null)
				return;

			_search = _navState.SearchAside;
			var spaceData = TfSpaceDataUIService.GetSpaceDataList(_navState.SpaceId.Value, _search).ToList();
			var dpDict = TfDataProviderUIService.GetDataProviders().ToDictionary(x=> x.Id);
			foreach (var sd in spaceData)
			{
				_items.Add(new TfMenuItem
				{
					Url = string.Format(TfConstants.SpaceDataPageUrl,_navState.SpaceId, sd.Id),
					Description = dpDict.ContainsKey(sd.DataProviderId) ?  dpDict[sd.DataProviderId].Name : "unknown",
					Text = TfConverters.StringOverflow(sd.Name, _stringLimit),
					Selected = _navState.SpaceDataId == sd.Id
				});
			}
		}
		finally
		{
			_isLoading = false;
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}
}