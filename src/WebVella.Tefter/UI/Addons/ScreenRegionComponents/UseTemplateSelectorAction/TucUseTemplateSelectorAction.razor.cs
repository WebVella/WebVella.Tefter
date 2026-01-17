namespace WebVella.Tefter.UI.Components;
public partial class TucUseTemplateSelectorAction : TfBaseComponent, ITfScreenRegionAddon<TfSpaceViewSelectorActionScreenRegion>
{
	//State
	public Guid AddonId { get; init; } = new Guid("3e344931-988c-4f84-874d-823e31ec73ad"); 
	public int PositionRank { get; init; } = 90;
	public string AddonName { get; init;} = "Use template with selection";
	public string AddonDescription { get; init;} = "";
	public string AddonFluentIconName { get; init; } =  "CalendarTemplate";
	public List<TfScreenRegionScope> Scopes { get; init; } = new ();

	[Parameter] 
	public TfSpaceViewSelectorActionScreenRegion RegionContext { get; set; } = null!;
	private async Task _clickHandler()
	{
		if (RegionContext.SelectedDataRows.Count == 0) return;

		var context = new TfUseTemplateContext
		{
			SelectedRowIds = RegionContext.SelectedDataRows.ToList(),
			SpaceData = RegionContext.Dataset,
			User = RegionContext.CurrentUser
		};
		_ = await DialogService.ShowDialogAsync<TucUseTemplateDialog>(
				context,
				new ()
				{
					PreventDismissOnOverlayClick = true,
					PreventScroll = true,
					Width = TfConstants.DialogWidthExtraLarge,
					TrapFocus = false
				});
	}		
}