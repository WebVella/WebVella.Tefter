namespace WebVella.Tefter.UI.Components;
public partial class TucBookmarkDataSelectorAction : TfBaseComponent, ITfScreenRegionAddon<TfSpaceViewSelectorActionScreenRegion>
{
	//State
	public Guid AddonId { get; init; } = new Guid("1ea2ccb8-4cdb-446f-9114-27d47a7615e6"); 
	public int PositionRank { get; init; } = 500;
	public string AddonName { get; init;} = "Pin Data in Home";
	public string AddonDescription { get; init;} = "";
	public string AddonFluentIconName { get; init; } =  "Pin";
	public List<TfScreenRegionScope> Scopes { get; init; } = new ();

	[Parameter] 
	public TfSpaceViewSelectorActionScreenRegion RegionContext { get; set; } = null!;
	private async Task _clickHandler()
	{
		if (RegionContext.SelectedDataRows.Count == 0) return;
		var context = new TfCreatePinDataBookmarkModel
		{
			SelectedRowIds = RegionContext.SelectedDataRows,
			Dataset = RegionContext.Dataset,
			User = RegionContext.CurrentUser,
			SpacePage = RegionContext.SpacePage,
			SpaceView = RegionContext.SpaceView,
		};
		_ = await DialogService.ShowDialogAsync<TucSpaceViewBookmarkPinDataDialog>(
				context,
				new()
				{
					PreventDismissOnOverlayClick = true,
					PreventScroll = true,
					Width = TfConstants.DialogWidthExtraLarge,
					TrapFocus = false
				});

	}		
}