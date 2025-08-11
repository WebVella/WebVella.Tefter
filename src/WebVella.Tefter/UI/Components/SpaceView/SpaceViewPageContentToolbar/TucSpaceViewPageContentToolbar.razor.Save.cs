namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewPageContentToolbar : TfBaseComponent
{
	private TucSpaceViewLinkSaveSelector _saveSelector;
	private async Task OnSaveLinkClick()
	{
		await _saveSelector.ToggleSelector();
	}
}