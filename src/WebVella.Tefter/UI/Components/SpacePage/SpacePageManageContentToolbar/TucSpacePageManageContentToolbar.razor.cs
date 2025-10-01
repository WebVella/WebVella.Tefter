namespace WebVella.Tefter.UI.Components;
public partial class TucSpacePageManageContentToolbar : TfBaseComponent, IDisposable
{
	private bool _isLoading = true;
	private List<TfMenuItem> _menu = new();
	private TfSpacePage? _spacePage = null;
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
			if (navState.SpacePageId is null)
				throw new Exception("Space page Id not found in URL");
			_spacePage = TfUIService.GetSpacePage(navState.SpacePageId.Value);
			var pageMeta = TfUIService.GetSpacePagesComponentsMeta();
			var component = pageMeta.FirstOrDefault(x => x.Instance.AddonId == _spacePage.ComponentId);
			if(_spacePage is null)
				return;
			var pageUrl = string.Format(TfConstants.SpacePagePageManageUrl, navState.SpaceId, navState.SpacePageId)
				.GenerateWithLocalAsReturnUrl(navState.ReturnUrl);
			var addonUrl = string.Format(TfConstants.SpacePagePageManageAddonUrl, navState.SpaceId, navState.SpacePageId)
				.GenerateWithLocalAsReturnUrl(navState.ReturnUrl);			
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = pageUrl,
				Selected = navState.NodesDict.Keys.Count == 5 && navState.HasNode(RouteDataNode.Manage, 4),
				Text = LOC("Page"),
				IconCollapsed = TfConstants.GetIcon(_spacePage.FluentIconName)
			});
			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = addonUrl,
				Selected = navState.NodesDict.Keys.Count == 5 && navState.HasNode(RouteDataNode.Addon, 4),
				Text = component is null ? LOC("Addon") : component.Instance.AddonName,
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