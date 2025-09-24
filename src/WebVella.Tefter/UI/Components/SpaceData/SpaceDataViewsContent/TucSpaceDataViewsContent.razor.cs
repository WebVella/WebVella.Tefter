//namespace WebVella.Tefter.UI.Components;
//public partial class TucSpaceDataViewsContent : TfBaseComponent, IDisposable
//{
//	[Inject] public ITfDatasetUIService TfDatasetUIService { get; set; } = default!;
//	[Inject] public ITfSpaceUIService TfSpaceUIService { get; set; } = default!;
//	[Inject] public ITfSpaceViewUIService TfSpaceViewUIService { get; set; } = default!;
//	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;

//	private TfDataset _spaceData = new();
//	private TfSpace _space = new();
//	private TfDataProvider _provider = new();
//	public bool _submitting = false;
//	public TfNavigationState? _navState = null;
//	private List<TfSpaceView> _spaceViewList = new();

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
//			_spaceViewList = TfSpaceViewUIService.GetAllSpaceViews().Where(x=> x.SpaceDataId == _spaceData.Id).ToList();

//		}
//		finally
//		{
//			UriInitialized = _navState.Uri;
//			await InvokeAsync(StateHasChanged);
//		}
//	}

	

//}