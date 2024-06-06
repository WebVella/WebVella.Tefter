namespace WebVella.Tefter.Demo.Components;
public partial class WvFastAccessNav : WvBaseComponent
{
	private List<MenuItem> _menuItems = new List<MenuItem>();
	private string _activeTabId = null;

	protected override void OnAfterRender(bool firstRender)
	{
		if(firstRender){
			generateTabs();
			StateHasChanged();
		}
	}

	private void generateTabs(){
		_menuItems.Add(new MenuItem
		{
			Id = "bookmarks",
			Title = "Bookmarks",
			Icon = WvConstants.BookmarkOFFIcon,
		});
		_menuItems.Add(new MenuItem
		{
			Id = "saves",
			Title = "Saved Views",
			Icon = WvConstants.SaveIcon,
		});
		var localPath = new Uri(Navigator.Uri).LocalPath.ToLowerInvariant();
		if(localPath == "/fast-access/saves")
			_activeTabId = "saves";
		else
			_activeTabId = "bookmarks";
	}

	private void HandleOnTabChange(FluentTab tab)
	{
		if(tab.Id == "bookmarks"){
			Navigator.NavigateTo($"/fast-access");
		}
		else{
			Navigator.NavigateTo($"/fast-access/{tab.Id}");
		}
		
	}
}