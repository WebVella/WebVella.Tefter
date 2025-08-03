namespace WebVella.Tefter.UI.Components;
public partial class TucSpacePageAsideToolbar : TfBaseComponent,IDisposable
{
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;
	private string? _search = null;
	private TfNavigationState _navState = new();
	private TfSpaceNavigationActiveTab _activeTab = TfSpaceNavigationActiveTab.Pages;

	public void Dispose()
	{
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
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
			_search = _navState.SearchAside;
			_activeTab = NavigatorExt.GetEnumFromQuery<TfSpaceNavigationActiveTab>(Navigator,TfConstants.TabQueryName,TfSpaceNavigationActiveTab.Pages)!.Value;
		}
		finally
		{
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}


	private async Task onSearch(string search)
	{
		_search = search;
		await NavigatorExt.ApplyChangeToUrlQuery(Navigator, TfConstants.AsideSearchQueryName, search);
	}

	private async Task _setActiveTab(TfSpaceNavigationActiveTab tab)
	{
		if (_activeTab == tab) return;
		_activeTab = tab;
		await NavigatorExt.ApplyChangeToUrlQuery(Navigator, TfConstants.TabQueryName, _activeTab);
	}


	
}