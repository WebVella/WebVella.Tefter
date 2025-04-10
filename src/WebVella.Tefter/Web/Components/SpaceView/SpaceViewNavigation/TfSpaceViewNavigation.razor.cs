using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewNavigation.TfSpaceViewNavigation", "WebVella.Tefter")]
public partial class TfSpaceViewNavigation : TfBaseComponent
{
	[Inject] protected IStateSelection<TfUserState, bool> SidebarExpanded { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private string search = null;
	protected override void OnInitialized()
	{
		base.OnInitialized();
		SidebarExpanded.Select(x => x.SidebarExpanded);
	}

	private List<TucMenuItem> _getMenu()
	{
		search = search?.Trim().ToLowerInvariant();
		var menuItems = new List<TucMenuItem>();
		var menuGroups = new List<string>();

		foreach (var record in TfAppState.Value.SpaceViewList.OrderBy(x => x.Name))
		{
			if (!String.IsNullOrWhiteSpace(search) && !record.Name.ToLowerInvariant().Contains(search))
				continue;

			var viewMenu = new TucMenuItem
			{
				Id = TfConverters.ConvertGuidToHtmlElementId(record.Id),
				IconCollapsed = TfConstants.SpaceViewIcon,
				Text = record.Name,
				Url = string.Format(TfConstants.SpaceViewPageUrl, record.SpaceId, record.Id),
				Selected = record.Id == TfAppState.Value.SpaceView?.Id
			};
			menuItems.Add(viewMenu);
		}

		return menuItems;
	}

	private async Task onAddClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSpaceViewManageDialog>(
		new TucSpaceView() with { SpaceId = TfAppState.Value.Space.Id },
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
			var resultObj = (Tuple<TucSpaceView, TucSpaceData>)result.Data;
			var spaceView = resultObj.Item1;
			var spaceData = resultObj.Item2;
			var viewList = TfAppState.Value.SpaceViewList.ToList();
			var dataList = TfAppState.Value.SpaceDataList.ToList();
			viewList.Add(spaceView);

			var dataIndex = dataList.FindIndex(x => x.Id == spaceData.Id);
			if (dataIndex > -1)
			{
				dataList[dataIndex] = spaceData;
			}
			else
			{
				dataList.Add(spaceData);
			}

			Dispatcher.Dispatch(new SetAppStateAction(
						component: this,
						state: TfAppState.Value with
						{
							SpaceView = spaceView,
							SpaceViewList = viewList.OrderBy(x => x.Position).ToList(),
							SpaceDataList = dataList.OrderBy(x => x.Position).ToList()
						}));

			ToastService.ShowSuccess(LOC("Space view successfully created!"));
			Navigator.NavigateTo(string.Format(TfConstants.SpaceViewPageUrl, spaceView.SpaceId, spaceView.Id));
		}
	}
	
	private void onSearch(string value)
	{
		search = value;
	}

}
