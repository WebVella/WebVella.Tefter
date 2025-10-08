// namespace WebVella.Tefter.UI.Components;
// public partial class TucSpaceViewDetailsContentToolbar : TfBaseComponent, IDisposable
// {
// 	private bool _isLoading = true;
// 	private List<TfMenuItem> _menu = new();
//
// 	public void Dispose()
// 	{
// 		TfState.NavigationStateChangedEvent -= On_NavigationStateChanged;
// 		TfUIService.SpaceViewUpdated -= On_SpaceViewUpdated;
// 		TfUIService.SpaceViewColumnsChanged -= On_SpaceViewColumnsUpdated;
// 	}
// 	protected override async Task OnInitializedAsync()
// 	{
// 		await _init(TfAuthLayout.AppState.NavigationState);
// 		TfState.NavigationStateChangedEvent += On_NavigationStateChanged;
// 		TfUIService.SpaceViewUpdated += On_SpaceViewUpdated;
// 		TfUIService.SpaceViewColumnsChanged += On_SpaceViewColumnsUpdated;
// 	}
// 	private async Task On_NavigationStateChanged(TfNavigationState args)
// 	{
// 		if (UriInitialized != args.Uri)
// 			await _init(args.Payload);
// 	}
// 	private async Task On_SpaceViewUpdated(object? caller, TfSpaceView args)
// 	{
// 		await _init(TfAuthLayout.AppState.NavigationState);
// 	}
//
// 	private async Task On_SpaceViewColumnsUpdated(object? caller, List<TfSpaceViewColumn> args)
// 	{
// 		await _init(TfAuthLayout.AppState.NavigationState);
// 	}
//
// 	private async Task _init(TfNavigationState navState)
// 	{
// 		try
// 		{
// 			_menu = new();
// 			if (navState.SpaceId is null || navState.SpaceViewId is null)
// 				return;
//
// 			var spaceView = TfUIService.GetSpaceView(navState.SpaceViewId.Value);
// 			if (spaceView is null)
// 				return;
// 			var spaceViewColumns = TfUIService.GetViewColumns(spaceView.Id);
// 			_menu.Add(new TfMenuItem
// 			{
// 				Id = Guid.NewGuid().ToString(),
// 				Url = string.Format(TfConstants.SpaceViewPageUrl, navState.SpaceId, navState.SpaceViewId),
// 				Selected = navState.NodesDict.Keys.Count == 4,
// 				Text = LOC("Details"),
// 				IconCollapsed = TfConstants.GetIcon("Info")
// 			});
// 			_menu.Add(new TfMenuItem
// 			{
// 				Id = Guid.NewGuid().ToString(),
// 				Url = string.Format(TfConstants.SpaceViewColumnsPageUrl, navState.SpaceId, navState.SpaceViewId),
// 				Selected = navState.HasNode(RouteDataNode.Columns, 4),
// 				Text = LOC("Columns"),
// 				IconCollapsed = TfConstants.GetIcon("Table"),
// 				BadgeContent = spaceViewColumns.Count == 0
// 					? null
// 					: builder =>
// 					{
// 						builder.OpenElement(0, "span");
// 						builder.AddContent(1, spaceViewColumns.Count);
// 						builder.CloseElement();
// 					}
// 			});
// 			_menu.Add(new TfMenuItem
// 			{
// 				Id = Guid.NewGuid().ToString(),
// 				Url = string.Format(TfConstants.SpaceViewFiltersPageUrl, navState.SpaceId, navState.SpaceViewId),
// 				Selected = navState.HasNode(RouteDataNode.Filters, 4),
// 				Text = LOC("Preset Filters"),
// 				IconCollapsed = TfConstants.GetIcon("Filter"),
// 				BadgeContent = spaceView.Presets.Count == 0
// 					? null
// 					: builder =>
// 					{
// 						builder.OpenElement(0, "span");
// 						builder.AddContent(1, spaceView.Presets.Count);
// 						builder.CloseElement();
// 					}
// 			});
// 			_menu.Add(new TfMenuItem
// 			{
// 				Id = Guid.NewGuid().ToString(),
// 				Url = string.Format(TfConstants.SpaceViewPagesPageUrl, navState.SpaceId, navState.SpaceViewId),
// 				Selected = navState.HasNode(RouteDataNode.Pages, 4),
// 				Text = LOC("Pages"),
// 				IconCollapsed = TfConstants.GetIcon("Document")
// 			});
// 		}
// 		finally
// 		{
// 			_isLoading = false;
// 			UriInitialized = navState.Uri;
// 			await InvokeAsync(StateHasChanged);
// 		}
// 	}
// }