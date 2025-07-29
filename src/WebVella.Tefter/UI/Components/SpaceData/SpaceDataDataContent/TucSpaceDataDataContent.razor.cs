namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceDataDataContent : TfBaseComponent, IDisposable
{
	[Inject] public ITfSpaceDataUIService TfSpaceDataUIService { get; set; } = default!;
	[Inject] public ITfDataProviderUIService TfDataProviderUIService { get; set; } = default!;
	[Inject] public ITfSpaceUIService TfSpaceUIService { get; set; } = default!;
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;

	private TfSpaceData _spaceData = new();
	private TfSpace _space = new();
	private TfDataProvider _provider = new();
	public bool _submitting = false;
	public TfNavigationState? _navState = null;
	private TfDataTable? _data = null;
	private int _page = 1;
	private int _pageSize = TfConstants.PageSize;
	public void Dispose()
	{
		TfSpaceDataUIService.SpaceDataUpdated -= On_SpaceDataUpdated;
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfSpaceDataUIService.SpaceDataUpdated += On_SpaceDataUpdated;
		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_SpaceDataUpdated(object? caller, TfSpaceData args)
	{
		await _init(spaceData: args);
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(navState: args);
	}

	private async Task _init(TfNavigationState? navState = null, TfSpaceData? spaceData = null)
	{
		if (navState == null)
			_navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		else
			_navState = navState;
		try
		{
			if (spaceData is not null && spaceData.Id == _spaceData?.Id)
			{
				_spaceData = spaceData;
			}
			else
			{
				var routeData = Navigator.GetRouteState();
				if (routeData.SpaceDataId is not null)
					_spaceData = TfSpaceDataUIService.GetSpaceData(routeData.SpaceDataId.Value);

			}
			if (_spaceData is null) return;
			_space = TfSpaceUIService.GetSpace(_spaceData.SpaceId);
			_page = _navState.Page ?? 1;
			_pageSize = _navState.PageSize ?? TfConstants.PageSize;
			_data = TfSpaceDataUIService.QuerySpaceData(
				spaceDataId:_spaceData.Id,
				userFilters:null,
				userSorts:null,
				presetFilters:null,
				presetSorts:null,
				search:_navState.Search,
				page: _page,
				pageSize: _pageSize
			);
			_page = _data.QueryInfo.Page ?? 1;
			_pageSize = _data.QueryInfo.PageSize ?? 1;

		}
		finally
		{
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

private async Task _goFirstPage()
	{
		if (_page == 1) return;
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.PageQueryName,1}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goPreviousPage()
	{
		var page =_page - 1;
		if (page < 1) page = 1;
		if (_page == page) return;
		var queryDict = new Dictionary<string, object?>{
			{ TfConstants.PageQueryName, page}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goNextPage()
	{
		if (_data is null
		|| _data.Rows.Count == 0)
			return;

		var page = _page + 1;
		if (page < 1) page = 1;
		if (_page == page) return;

		var queryDict = new Dictionary<string, object>{
			{ TfConstants.PageQueryName, page}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goLastPage()
	{
		if (_page == -1) return;
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.PageQueryName, -1}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goOnPage(int page)
	{
		if (page < 1 && page != -1) page = 1;
		if (_page == page) return;
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.PageQueryName, page}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}


	private async Task _onSearch(string value)
	{
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.SearchQueryName, value}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}	

}