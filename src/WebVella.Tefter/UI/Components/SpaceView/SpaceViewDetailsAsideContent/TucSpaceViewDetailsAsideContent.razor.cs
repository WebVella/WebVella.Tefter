namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewDetailsAsideContent : TfBaseComponent, IDisposable
{
	[Inject] public ITfDataProviderUIService TfDataProviderUIService { get; set; } = default!;
	[Inject] public ITfSpaceViewUIService TfSpaceViewUIService { get; set; } = default!;
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;

	private bool _isLoading = true;
	private int _stringLimit = 30;
	private string? _search = String.Empty;
	private List<TfMenuItem> _items = new();
	private TfNavigationState _navState = new();
	public void Dispose()
	{
		TfSpaceViewUIService.SpaceViewCreated -= On_SpaceViewChanged;
		TfSpaceViewUIService.SpaceViewUpdated -= On_SpaceViewChanged;
		TfSpaceViewUIService.SpaceViewDeleted -= On_SpaceViewChanged;
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfSpaceViewUIService.SpaceViewCreated += On_SpaceViewChanged;
		TfSpaceViewUIService.SpaceViewUpdated += On_SpaceViewChanged;
		TfSpaceViewUIService.SpaceViewDeleted += On_SpaceViewChanged;
		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_SpaceViewChanged(object? caller, TfSpaceView args)
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
			var spaceViews = TfSpaceViewUIService.GetSpaceViewsList(_navState.SpaceId.Value, _search).ToList();
			var dpDict = TfDataProviderUIService.GetDataProviders().ToDictionary(x=> x.Id);
			foreach (var sd in spaceViews)
			{
				_items.Add(new TfMenuItem
				{
					Url = string.Format(TfConstants.SpaceViewPageUrl,_navState.SpaceId, sd.Id),
					Description = !String.IsNullOrWhiteSpace(sd.SpaceDataName) ?  sd.SpaceDataName : "unknown",
					Text = TfConverters.StringOverflow(sd.Name, _stringLimit),
					Selected = _navState.SpaceViewId == sd.Id
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