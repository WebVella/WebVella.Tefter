namespace WebVella.Tefter.UI.Components;
public partial class TucSpacePageManageContentToolbar : TfBaseComponent, IDisposable
{
	private bool _isLoading = true;
	private List<TfMenuItem> _menu = new();

	public void Dispose()
	{
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
	}
	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}
	private async Task _init(TfNavigationState? navState = null)
	{
		if (navState is null)
			navState = TfAuthLayout.NavigationState;
		try
		{
			_menu = new();
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.SpacePagePageManageUrl, navState.SpaceId, navState.SpacePageId),
				Selected = navState.NodesDict.Keys.Count == 5 && navState.HasNode(RouteDataNode.Manage, 4),
				Text = LOC("Details"),
				IconCollapsed = TfConstants.GetIcon("Info")
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.SpacePagePageManageAddonUrl, navState.SpaceId, navState.SpacePageId),
				Selected = navState.NodesDict.Keys.Count == 5 && navState.HasNode(RouteDataNode.Addon, 4),
				Text = LOC("Addon"),
				IconCollapsed = TfConstants.GetIcon("PlugConnected")
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