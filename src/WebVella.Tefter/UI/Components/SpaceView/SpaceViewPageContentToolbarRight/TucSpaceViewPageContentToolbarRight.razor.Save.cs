namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewPageContentToolbarRight : TfBaseComponent
{
	private TucSpaceViewLinkSaveSelector _saveSelector;
	private async Task OnSaveLinkClick()
	{
		await _saveSelector.ToggleSelector();
	}
}