namespace WebVella.Tefter.UI.Components;
public partial class TucSpacePageManageContentToolbar : TfBaseComponent, IDisposable
{
	private bool _isLoading = true;
	private List<TfMenuItem> _menu = new();
	private TfSpacePage? _spacePage = null;
	public void Dispose()
	{
		TfAuthLayout.NavigationStateChangedEvent -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.NavigationState);
		TfAuthLayout.NavigationStateChangedEvent += On_NavigationStateChanged;
	}
	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}
	private async Task _init(TfNavigationState navState)
	{
		try
		{
			_menu = new();
			if (navState.SpacePageId is null)
				throw new Exception("Space page Id not found in URL");
			_spacePage = TfService.GetSpacePage(navState.SpacePageId.Value);
			var pageMeta = TfMetaService.GetSpacePagesComponentsMeta();
			var component = pageMeta.FirstOrDefault(x => x.Instance.AddonId == _spacePage.ComponentId);
			if(_spacePage is null)
				return;

			_menu.Add(new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Url = string.Format(TfConstants.SpacePagePageManageUrl, navState.SpaceId, navState.SpacePageId)
					.GenerateWithLocalAsReturnUrl(navState.ReturnUrl),
				Selected = navState.NodesDict.Keys.Count == 5 && navState.HasNode(RouteDataNode.Manage, 4),
				Text = LOC("Page"),
				IconCollapsed = TfConstants.GetIcon(_spacePage.FluentIconName)
			});
			var tabs = component.Instance.GetManagementTabs();
			foreach (var tab in component.Instance.GetManagementTabs())
			{
				_menu.Add(new TfMenuItem
				{
					Id = Guid.NewGuid().ToString(),
					Url = string.Format(TfConstants.SpacePagePageManageTabUrl, navState.SpaceId, navState.SpacePageId, tab.Slug)
						.GenerateWithLocalAsReturnUrl(navState.ReturnUrl),
					Selected = navState.NodesDict.Keys.Count == 5 && navState.HasNode(RouteDataNode.ManageTab, 4) && navState.ManageTab == tab.Slug,
					Text = tab.Label,
					IconCollapsed = TfConstants.GetIcon(String.IsNullOrWhiteSpace(tab.FluentIconName) ? "PlugConnected" : tab.FluentIconName)
				});				
			}

		}
		finally
		{
			_isLoading = false;
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}
}