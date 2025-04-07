namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceData.SpaceDataNavigation.TfSpaceDataNavigation", "WebVella.Tefter")]
public partial class TfSpaceDataNavigation : TfBaseComponent
{
	[Inject] protected IStateSelection<TfUserState, bool> SidebarExpanded { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
		SidebarExpanded.Select(x => x.SidebarExpanded);
	}

	private bool _settingsMenuVisible = false;
	private string search = null;

	private List<TucMenuItem> _getMenu()
	{
		search = search?.Trim().ToLowerInvariant();
		var menuItems = new List<TucMenuItem>();
		foreach (var spaceData in TfAppState.Value.SpaceDataList.OrderBy(x => x.Name))
		{
			if (!String.IsNullOrWhiteSpace(search) && !spaceData.Name.ToLowerInvariant().Contains(search))
				continue;

			var menu = new TucMenuItem
			{
				Id = TfConverters.ConvertGuidToHtmlElementId(spaceData.Id),
				IconCollapsed = TfConstants.SpaceDataIcon,
				Text = spaceData.Name,
				Url = String.Format(TfConstants.SpaceDataPageUrl, spaceData.SpaceId, spaceData.Id),
				Selected = spaceData.Id == TfAppState.Value.Route.SpaceDataId
			};
			menuItems.Add(menu);
		}

		return menuItems;
	}

	private async Task onAddClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSpaceDataManageDialog>(
		new TucSpaceData { SpaceId = TfAppState.Value.Space.Id },
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var item = (TucSpaceData)result.Data;
			ToastService.ShowSuccess(LOC("Space dataset successfully created!"));

			var itemList = TfAppState.Value.SpaceDataList.ToList();
			itemList.Add(item);

			Dispatcher.Dispatch(new SetAppStateAction(
			component: this,
			state: TfAppState.Value with
			{
				SpaceData = item,
				SpaceDataList = itemList.OrderBy(x => x.Position).ToList()
			}));

			Navigator.NavigateTo(String.Format(TfConstants.SpaceDataPageUrl, item.SpaceId, item.Id));
		}
	}

	private void onSearch(string value)
	{
		search = value;
	}
}