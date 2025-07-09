namespace WebVella.Tefter.UI.Components;
public partial class TucAdminDataProviderDetailsContentToolbar : TfBaseComponent, IDisposable
{
	[Inject] public ITfSpaceUIService TfSpaceUIService { get; set; } = default!;

	private bool _isLoading = true;
	private List<TfMenuItem> _menu = new();

	public void Dispose()
	{
		TfSpaceUIService.NavigationDataChanged -= On_NavigationDataChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfSpaceUIService.NavigationDataChanged += On_NavigationDataChanged;
	}
	private async void On_NavigationDataChanged(object? caller, TfSpaceNavigationData args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}

	private async Task _init(TfSpaceNavigationData? navData = null)
	{
		if (navData is null)
			navData = await TfSpaceUIService.GetSpaceNavigationData(Navigator);
		try
		{
			_menu = new();
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.AdminDataProviderDetailsPageUrl, navData.State.DataProviderId),
				Selected = navData.State.NodesDict.Keys.Count == 3 || navData.State.HasNode(RouteDataNode.Details, 3),
				Text = LOC("Details"),
				IconCollapsed = TfConstants.GetIcon("Info")
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.AdminDataProviderSchemaPageUrl, navData.State.DataProviderId),
				Selected = navData.State.HasNode(RouteDataNode.Schema, 3),
				Text = LOC("Provider Columns"),
				IconCollapsed = TfConstants.GetIcon("Table")
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.AdminDataProviderAuxPageUrl, navData.State.DataProviderId),
				Selected = navData.State.HasNode(RouteDataNode.Aux, 3),
				Text = LOC("Connected Data"),
				IconCollapsed = TfConstants.GetIcon("PlugConnected")
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.AdminDataProviderSynchronizationPageUrl, navData.State.DataProviderId),
				Selected = navData.State.HasNode(RouteDataNode.Synchronization, 3),
				Text = LOC("Synchronization"),
				IconCollapsed = TfConstants.GetIcon("ArrowSync")
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.AdminDataProviderDataPageUrl, navData.State.DataProviderId),
				Selected = navData.State.HasNode(RouteDataNode.Data, 3),
				Text = LOC("Data"),
				IconCollapsed = TfConstants.GetIcon("Database")
			});
		}
		finally
		{
			_isLoading = false;
			UriInitialized = navData.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}
}