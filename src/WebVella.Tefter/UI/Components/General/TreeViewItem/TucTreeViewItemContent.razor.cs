namespace WebVella.Tefter.UI.Components;
public partial class TucTreeViewItemContent : ComponentBase
{
	[Inject] protected NavigationManager Navigator { get; set; } = null!;
	[Parameter] public TfMenuItem Item { get; set; } = null!;
	
	[Parameter] public EventCallback<TfMenuItem> OnExpand { get; set; } = default!;

	private async Task _onExpand()
	{
		await OnExpand.InvokeAsync();
	}
}