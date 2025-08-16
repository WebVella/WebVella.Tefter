namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewPageContentToolbar : TfBaseComponent
{
	[Parameter] public EventCallback OnClearPersonalization { get; set; }
	private async Task OnClearPersonalizationClick()
	{
		await OnClearPersonalization.InvokeAsync();
	}
}