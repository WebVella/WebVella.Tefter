namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewActionSelector : TfBaseComponent
{
	[Parameter] public TfSpaceView SpaceView { get; set; } = null!;
	[Parameter] public TfDataset SpaceData { get; set; } = null!;
	[Parameter] public TfUser User { get; set; } = null!;
	[Parameter] public TfDataTable Data { get; set; } = null!;
	[Parameter] public List<Guid> SelectedRows { get; set; } = new();

	[CascadingParameter(Name = "TucSpaceViewPageContent")]
	public TucSpaceViewPageContent TucSpaceViewPageContent { get; set; } = null!;

	private bool _open = false;
	private TfSpaceViewSelectorActionScreenRegion _context
	{
		get
		{
			return new TfSpaceViewSelectorActionScreenRegion()
			{
				SelectedDataRows = SelectedRows,
				SpaceData = SpaceData,
				CurrentUser = User,
				TucSpaceViewPageContent = TucSpaceViewPageContent
			};
		}
	}
	public async Task ToggleSelector()
	{
		_open = !_open;
		await InvokeAsync(StateHasChanged);
	}

	private async Task _deleteSelectedRecords()
	{
		if (SelectedRows.Count == 0) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need these records deleted?")))
			return;
		TucSpaceViewPageContent.OnDeleteRows(SelectedRows);
	}
}