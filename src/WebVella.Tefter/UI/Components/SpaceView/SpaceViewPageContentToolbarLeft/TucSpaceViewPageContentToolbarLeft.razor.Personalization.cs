namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewPageContentToolbarLeft : TfBaseComponent
{
	private async Task OnClearPersonalizationClick()
	{
		await TucSpaceViewPageContent.OnClearPersonalization();
	}
}