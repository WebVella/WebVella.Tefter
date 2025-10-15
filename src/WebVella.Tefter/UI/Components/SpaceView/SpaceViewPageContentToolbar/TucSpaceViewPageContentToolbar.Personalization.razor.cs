namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewPageContentToolbar
{
	private async Task OnClearPersonalizationClick()
	{
		await TucSpaceViewPageContent.OnClearPersonalization();
	}
}