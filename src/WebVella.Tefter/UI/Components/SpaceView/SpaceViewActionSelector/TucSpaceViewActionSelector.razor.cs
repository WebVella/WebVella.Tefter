namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewActionSelector : TfBaseComponent
{
	[Inject] public ITfSpaceDataUIService TfSpaceDataUIService { get; set; } = default!;
	[Inject] public ITfDataProviderUIService TfDataProviderUIService { get; set; } = default!;
	[Parameter] public TfSpaceView SpaceView { get; set; } = default!;
	[Parameter] public TfSpaceData SpaceData { get; set; } = default!;
	[Parameter] public TfUser User { get; set; } = default!;
	[Parameter] public TfDataTable Data { get; set; } = default!;
	[Parameter] public List<Guid> SelectedRows { get; set; } = new();

	private bool _open = false;
	private TfSpaceViewSelectorActionScreenRegionContext _context
	{
		get
		{
			return new TfSpaceViewSelectorActionScreenRegionContext()
			{
				SelectedDataRows = SelectedRows,
				SpaceData = SpaceData,
				CurrentUser = User
			};
		}
	}
	public async Task ToggleSelector()
	{
		_open = !_open;
		await InvokeAsync(StateHasChanged);
	}

	private Task _deleteSelectedRecords()
	{
		if (SelectedRows.Count == 0) return Task.CompletedTask;
		try
		{
			var spaceData = TfSpaceDataUIService.GetSpaceData(SpaceView.SpaceDataId);
			TfDataProviderUIService.DeleteDataProviderRowsByTfId(
				providerId: spaceData.DataProviderId,
				idList: SelectedRows
			);
			ToastService.ShowSuccess(LOC("Records deleted"));
			Navigator.ReloadCurrentUrl();
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		return Task.CompletedTask;
	}
}