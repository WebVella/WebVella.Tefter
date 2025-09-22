namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewPageContentToolbarLeft : TfBaseComponent
{
	[Parameter] public bool SelectAllLoading { get; set; } = false;
	[Parameter] public List<Guid> SelectedRows { get; set; } = new();
	private bool _selectAllMenuVisible { get; set; } = false;

	private async Task OnSelectAllClick()
	{
		await TucSpaceViewPageContent.OnSelectAll();
	}

	private async Task OnDeSelectAllClick()
	{
		await TucSpaceViewPageContent.OnDeSelectAll();
	}
}