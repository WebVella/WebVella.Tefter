namespace WebVella.Tefter.UI.Components;

public partial class TucAdminDataProviderDetailsContentToolbar : TfBaseComponent, IAsyncDisposable
{
	private bool _isLoading = true;
	private List<TfMenuItem> _menu = new();
	private IAsyncDisposable _dataProviderUpdatedEventSubscriber = null!;

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		await _dataProviderUpdatedEventSubscriber.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		_dataProviderUpdatedEventSubscriber = await TfEventBus.SubscribeAsync<TfDataProviderUpdatedEventPayload>(
			handler: On_DataProviderUpdatedEventAsync);
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
				await _init(TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task On_DataProviderUpdatedEventAsync(string? key, TfDataProviderUpdatedEventPayload? payload)
		=> await _init(TfAuthLayout.GetState().NavigationState);

	private async Task _init(TfNavigationState navState)
	{
		try
		{
			_menu = new();
			if (navState.DataProviderId is null) return;
			var provider = TfService.GetDataProvider(navState.DataProviderId.Value);
			if (provider is null) return;
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.AdminDataProviderDetailsPageUrl, navState.DataProviderId),
				Selected = navState.NodesDict.Keys.Count == 3 || navState.HasNode(RouteDataNode.Details, 3),
				Text = LOC("Details"),
				//IconCollapsed = TfConstants.GetIcon("Info")
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.AdminDataProviderSchemaPageUrl, navState.DataProviderId),
				Selected = navState.HasNode(RouteDataNode.Schema, 3),
				Text = LOC("Columns"),
				//IconCollapsed = TfConstants.GetIcon("Table"),
				BadgeContent = provider.Columns.Count == 0
					? null
					: builder =>
					{
						builder.OpenElement(0, "span");
						builder.AddContent(1, provider.Columns.Count);
						builder.CloseElement();
					}
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.AdminDataProviderAuxPageUrl, navState.DataProviderId),
				Selected = navState.HasNode(RouteDataNode.Aux, 3),
				Text = LOC("Connected Data"),
				//IconCollapsed = TfConstants.GetIcon("PlugConnected")
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.AdminDataProviderSynchronizationPageUrl, navState.DataProviderId),
				Selected = navState.HasNode(RouteDataNode.Synchronization, 3),
				Text = LOC("Synch"),
				//IconCollapsed = TfConstants.GetIcon("ArrowSync")
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.AdminDataProviderDataPageUrl, navState.DataProviderId),
				Selected = navState.HasNode(RouteDataNode.Data, 3),
				Text = LOC("Data"),
				//IconCollapsed = TfConstants.GetIcon("Database")
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.AdminDataProviderDatasetsPageUrl, navState.DataProviderId),
				Selected = navState.HasNode(RouteDataNode.Datasets, 3),
				Text = LOC("Datasets"),
				//IconCollapsed = TfConstants.GetIcon("DatabaseWindow")
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