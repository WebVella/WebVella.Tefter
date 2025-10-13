namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewPageContentToolbar : TfBaseComponent
{
	private async Task OnClearPersonalizationClick()
	{
		await TucSpaceViewPageContent.OnClearPersonalization();
	}
}