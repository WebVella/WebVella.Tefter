namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewToolbar : TfBaseComponent
{
	[Parameter] public EventCallback OnSelectAll { get; set; }
	[Parameter] public EventCallback OnDeSelectAll { get; set; }

	[Parameter] public bool SelectAllLoading { get; set; } = false;
	private bool _selectAllMenuVisible { get; set; } = false;

	private async Task OnSelectAllClick()
	{
		await OnSelectAll.InvokeAsync();
	}

	private async Task OnDeSelectAllClick()
	{
		await OnDeSelectAll.InvokeAsync();
	}

}