namespace WebVella.Tefter.Demo.Components;
public partial class WvSidebarToggle : WvBaseComponent
{
	private bool _isExpanded = true;

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if(firstRender){
			_isExpanded = WvState.GetUiSettings().SidebarExpanded;
			StateHasChanged();
		}
	}
	
	private async Task _toggle(){
		await WvState.ToggleSidebar();
		_isExpanded = WvState.GetUiSettings().SidebarExpanded;
	}
}