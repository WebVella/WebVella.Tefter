namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewDetailsContentToolbar : TfBaseComponent, IDisposable
{
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;

	private bool _isLoading = true;
	private List<TfMenuItem> _menu = new();

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
			navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		try
		{
			_menu = new();
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.SpaceViewPageUrl, navState.SpaceId, navState.SpaceViewId),
				Selected = navState.NodesDict.Keys.Count == 4,
				Text = LOC("Details"),
				IconCollapsed = TfConstants.GetIcon("Info")
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url =  string.Format(TfConstants.SpaceViewColumnsPageUrl, navState.SpaceId, navState.SpaceViewId),
				Selected = navState.HasNode(RouteDataNode.Columns, 4),
				Text = LOC("Columns"),
				IconCollapsed = TfConstants.GetIcon("Table")
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.SpaceViewFiltersPageUrl, navState.SpaceId, navState.SpaceViewId),
				Selected = navState.HasNode(RouteDataNode.Filters,4),
				Text = LOC("Preset Filters"),
				IconCollapsed = TfConstants.GetIcon("Filter")
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.SpaceViewPagesPageUrl,navState.SpaceId, navState.SpaceViewId),
				Selected = navState.HasNode(RouteDataNode.Pages, 4),
				Text = LOC("Pages"),
				IconCollapsed = TfConstants.PageIcon
			});
		}
		finally
		{
			_isLoading = false;
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}
}