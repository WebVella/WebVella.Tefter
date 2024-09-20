namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewToolbar : TfBaseComponent
{

	//Filter
	private TfSpaceViewLinkSaveSelector _saveSelector;
	private async Task OnSaveLinkClick()
	{
		await _saveSelector.ToggleSelector();
	}

}