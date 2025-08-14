namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceDataDetailsContentToolbar : TfBaseComponent, IDisposable
{
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;
	[Inject] public ITfSpaceDataUIService TfSpaceDataUIService { get; set; } = default!;

	private bool _isLoading = true;
	private List<TfMenuItem> _menu = new();

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
	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}
	private async void On_SpaceDataUpdated(object? caller, TfSpaceData args)
	{
		await _init(null);
	}
	private async Task _init(TfNavigationState? navState = null)
	{
		if (navState is null)
			navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		try
		{
			_menu = new();
			if (navState is null || navState.SpaceId is null || navState.SpaceDataId is null)
				return;

			var spaceData = TfSpaceDataUIService.GetSpaceData(navState.SpaceDataId.Value);
			if (spaceData is null)
				return;

			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.SpaceDataPageUrl, navState.SpaceId, navState.SpaceDataId),
				Selected = navState.NodesDict.Keys.Count == 4,
				Text = LOC("Details"),
				IconCollapsed = TfConstants.GetIcon("Info")
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.SpaceDataColumnsPageUrl, navState.SpaceId, navState.SpaceDataId),
				Selected = navState.HasNode(RouteDataNode.Columns, 4),
				Text = LOC("Columns"),
				IconCollapsed = TfConstants.GetIcon("Table"),
				BadgeCount = spaceData.Columns.Count == 0 ? null : spaceData.Columns.Count
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.SpaceDataFiltersPageUrl, navState.SpaceId, navState.SpaceDataId),
				Selected = navState.HasNode(RouteDataNode.Filters, 4),
				Text = LOC("Filters"),
				IconCollapsed = TfConstants.GetIcon("Filter"),
				BadgeCount = spaceData.Filters.Count == 0 ? null : spaceData.Filters.Count
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.SpaceDataSortsPageUrl, navState.SpaceId, navState.SpaceDataId),
				Selected = navState.HasNode(RouteDataNode.Sorts, 4),
				Text = LOC("Sort Order"),
				IconCollapsed = TfConstants.GetIcon("ArrowSortDownLines"),
				BadgeCount = spaceData.SortOrders.Count == 0 ? null : spaceData.SortOrders.Count
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.SpaceDataConnectedDataPageUrl, navState.SpaceId, navState.SpaceDataId),
				Selected = navState.HasNode(RouteDataNode.Aux, 4),
				Text = LOC("Connected Data"),
				IconCollapsed = TfConstants.GetIcon("PlugConnected")
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.SpaceDataViewsPageUrl, navState.SpaceId, navState.SpaceDataId),
				Selected = navState.HasNode(RouteDataNode.Views, 4),
				Text = LOC("Used in Views"),
				IconCollapsed = TfConstants.GetIcon("Document")
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.SpaceDataDataPageUrl, navState.SpaceId, navState.SpaceDataId),
				Selected = navState.HasNode(RouteDataNode.Data, 4),
				Text = LOC("Data"),
				IconCollapsed = TfConstants.GetIcon("Database")
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