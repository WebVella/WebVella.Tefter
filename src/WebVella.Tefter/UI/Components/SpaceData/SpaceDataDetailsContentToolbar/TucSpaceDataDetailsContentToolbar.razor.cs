namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceDataDetailsContentToolbar : TfBaseComponent, IDisposable
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
				Url = string.Format(TfConstants.SpaceDataPageUrl, navState.SpaceId, navState.SpaceDataId),
				Selected = navState.NodesDict.Keys.Count == 4,
				Text = LOC("Details"),
				IconCollapsed = TfConstants.GetIcon("Info")
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url =  string.Format(TfConstants.SpaceDataConnectedDataPageUrl, navState.SpaceId, navState.SpaceDataId),
				Selected = navState.HasNode(RouteDataNode.Aux, 4),
				Text = LOC("Connected Data"),
				IconCollapsed = TfConstants.GetIcon("Table")
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.SpaceDataViewsPageUrl, navState.SpaceId, navState.SpaceDataId),
				Selected = navState.HasNode(RouteDataNode.Views,4),
				Text = LOC("Used in Views"),
				IconCollapsed = TfConstants.GetIcon("PlugConnected")
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.SpaceDataDataPageUrl,navState.SpaceId, navState.SpaceDataId),
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