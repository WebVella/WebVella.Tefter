namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewPageContentToolbarRight : TfBaseComponent
{
	private TucPageLinkSaveSelector _saveSelector;
	private async Task OnSaveLinkClick()
	{
		await _saveSelector.ToggleSelector();
	}
}