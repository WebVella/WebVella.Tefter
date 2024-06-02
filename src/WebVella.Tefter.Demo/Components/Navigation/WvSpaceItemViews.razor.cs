namespace WebVella.Tefter.Demo.Components;
public partial class WvSpaceItemViews : WvBaseComponent
{
	private Space space = null;
	private SpaceItem spaceItem = null;
	private SpaceItemView spaceItemView = null;
	private List<MenuItem> _menuItems = new List<MenuItem>();
	private string _activeTabId = null;

	protected override void OnInitialized()
	{
		(space,spaceItem,spaceItemView) = WvState.GetActiveSpaceData();
		foreach (var view in spaceItem.Views)
		{
			var menu = new MenuItem
			{
				Id = RenderUtils.ConvertGuidToHtmlElementId(view.Id),
				Title = view.Name
			};
			if(view.Id == spaceItem.MainViewId){ 
				menu.Url = $"/space/{space.Id}/item/{spaceItem.Id}";
			}
			else {
				menu.Url = $"/space/{space.Id}/item/{spaceItem.Id}/view/{view.Id}";
			}
			_menuItems.Add(menu);
		}

		_activeTabId = RenderUtils.ConvertGuidToHtmlElementId(spaceItemView.Id);
	}

	private void HandleOnTabChange(FluentTab tab)
	{
		
		if(tab.Id == RenderUtils.ConvertGuidToHtmlElementId(spaceItem.MainViewId))
		{ 
			Navigator.NavigateTo($"/space/{space.Id}/item/{spaceItem.Id}");
		}
		else{
			Navigator.NavigateTo($"/space/{space.Id}/item/{spaceItem.Id}/view/{RenderUtils.ConvertHtmlElementIdToGuid(tab.Id)}");
		}
	}
}