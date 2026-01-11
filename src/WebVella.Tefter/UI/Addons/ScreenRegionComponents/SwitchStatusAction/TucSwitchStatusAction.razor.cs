namespace WebVella.Tefter.UI.Components;
public partial class TucSwitchStatusAction : TfBaseComponent, ITfScreenRegionAddon<TfSpaceViewToolBarActionScreenRegion>
{
	//State
	public Guid AddonId { get; init; } = new Guid("62fd82b3-77a2-43ab-829c-aae76e5441f5"); 
	public int PositionRank { get; init; } = 500;
	public string AddonName { get; init;} = "Change status";
	public string AddonDescription { get; init;} = "";
	public string AddonFluentIconName { get; init; } =  "CubeTree";
	public List<TfScreenRegionScope> Scopes { get; init; } = new ();

	[Parameter] 
	public TfSpaceViewToolBarActionScreenRegion RegionContext { get; set; } = null!;
	private async Task _clickHandler()
	{
		if (RegionContext.SelectedDataRows.Count == 0) return;
		var context = new TucChangeStatusDialogModel
		{
			SelectedRowIds = RegionContext.SelectedDataRows,
			Data =  RegionContext.Data,
			Dataset = RegionContext.Dataset,
			User = RegionContext.CurrentUser,
			SpacePage = RegionContext.SpacePage,
			SpaceView = RegionContext.SpaceView,
		};
		_ = await DialogService.ShowDialogAsync<TucChangeStatusDialog>(
				context,
				new()
				{
					PreventDismissOnOverlayClick = true,
					PreventScroll = true,
					//Width = TfConstants.DialogWidthExtraLarge,
					TrapFocus = false
				});
	}		
}