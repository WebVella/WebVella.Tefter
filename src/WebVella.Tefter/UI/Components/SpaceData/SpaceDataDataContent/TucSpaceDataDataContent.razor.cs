//namespace WebVella.Tefter.UI.Components;

//public partial class TucSpaceDataDataContent : TfBaseComponent, IDisposable
//{
//	[Inject] public ITfDatasetUIService TfDatasetUIService { get; set; } = default!;
//	[Inject] public ITfDataProviderUIService TfDataProviderUIService { get; set; } = default!;
//	[Inject] public ITfSpaceUIService TfSpaceUIService { get; set; } = default!;
//	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;
//	[Inject] private ITfUserUIService TfUserUIService { get; set; } = default!;
//	[CascadingParameter(Name = "CurrentUser")] public TfUser CurrentUser { get; set; } = default!;

//	private TfDataset _spaceData = new();
//	private TfSpace _space = new();
//	private TfDataProvider _provider = new();
//	public bool _submitting = false;
//	public TfNavigationState? _navState = null;
//	private TfDataTable? _data = null;

//	public void Dispose()
//	{
//		TfSpaceDataUIService.SpaceDataUpdated -= On_SpaceDataUpdated;
//		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
//	}

//	protected override async Task OnInitializedAsync()
//	{
//		await _init();
//		TfSpaceDataUIService.SpaceDataUpdated += On_SpaceDataUpdated;
//		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
//	}

//	private async void On_SpaceDataUpdated(object? caller, TfDataset args)
//	{
//		await _init(spaceData: args);
//	}

//	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
//	{
//		if (UriInitialized != args.Uri)
//			await _init(navState: args);
//	}

//	private async Task _init(TfNavigationState? navState = null, TfDataset? spaceData = null)
//	{
//		if (navState == null)
//			_navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
//		else
//			_navState = navState;
//		try
//		{
//			if (spaceData is not null && spaceData.Id == _spaceData?.Id)
//			{
//				_spaceData = spaceData;
//			}
//			else
//			{
//				var routeData = Navigator.GetRouteState();
//				if (routeData.SpaceDataId is not null)
//					_spaceData = TfSpaceDataUIService.GetSpaceData(routeData.SpaceDataId.Value);

//			}
//			if (_spaceData is null) return;
//			_space = TfSpaceUIService.GetSpace(_spaceData.SpaceId);
//			_data = TfSpaceDataUIService.QuerySpaceData(
//				spaceDataId: _spaceData.Id,
//				userFilters: null,
//				userSorts: null,
//				presetFilters: null,
//				presetSorts: null,
//				search: _navState.Search,
//				page: 1,
//				pageSize: TfConstants.ItemsMaxLimit
//			);
//		}
//		finally
//		{
//			UriInitialized = _navState.Uri;
//			await InvokeAsync(StateHasChanged);
//		}
//	}

//	private async Task _onSearch(string value)
//	{
//		var queryDict = new Dictionary<string, object>{
//			{ TfConstants.SearchQueryName, value}
//		};
//		await Navigator.ApplyChangeToUrlQuery(queryDict);
//	}

//}