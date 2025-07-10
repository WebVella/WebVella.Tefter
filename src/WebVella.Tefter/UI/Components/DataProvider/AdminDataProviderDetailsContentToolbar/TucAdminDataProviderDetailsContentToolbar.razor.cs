namespace WebVella.Tefter.UI.Components;
public partial class TucAdminDataProviderDetailsContentToolbar : TfBaseComponent, IDisposable
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
			navState = await TfNavigationUIService.GetNavigationState(Navigator);
		try
		{
			_menu = new();
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.AdminDataProviderDetailsPageUrl, navState.DataProviderId),
				Selected = navState.NodesDict.Keys.Count == 3 || navState.HasNode(RouteDataNode.Details, 3),
				Text = LOC("Details"),
				IconCollapsed = TfConstants.GetIcon("Info")
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.AdminDataProviderSchemaPageUrl, navState.DataProviderId),
				Selected = navState.HasNode(RouteDataNode.Schema, 3),
				Text = LOC("Provider Columns"),
				IconCollapsed = TfConstants.GetIcon("Table")
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.AdminDataProviderAuxPageUrl, navState.DataProviderId),
				Selected = navState.HasNode(RouteDataNode.Aux, 3),
				Text = LOC("Connected Data"),
				IconCollapsed = TfConstants.GetIcon("PlugConnected")
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.AdminDataProviderSynchronizationPageUrl, navState.DataProviderId),
				Selected = navState.HasNode(RouteDataNode.Synchronization, 3),
				Text = LOC("Synchronization"),
				IconCollapsed = TfConstants.GetIcon("ArrowSync")
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.AdminDataProviderDataPageUrl, navState.DataProviderId),
				Selected = navState.HasNode(RouteDataNode.Data, 3),
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