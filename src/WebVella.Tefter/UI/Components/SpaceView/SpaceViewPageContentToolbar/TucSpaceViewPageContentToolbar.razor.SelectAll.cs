namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewPageContentToolbar : TfBaseComponent
{
	[Parameter] public EventCallback OnSelectAll { get; set; }
	[Parameter] public EventCallback OnDeSelectAll { get; set; }

	[Parameter] public bool SelectAllLoading { get; set; } = false;
	[Parameter] public List<Guid> SelectedRows { get; set; } = new();
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