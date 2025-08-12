namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.UseTemplateSelectorAction.TfUseTemplateSelectorAction", "WebVella.Tefter")]

public partial class TucUseTemplateSelectorAction : TfBaseComponent, ITfScreenRegionComponent<TfSpaceViewSelectorActionScreenRegionContext>
{
	//State
	public Guid AddonId { get; init; } = new Guid("3e344931-988c-4f84-874d-823e31ec73ad"); 
	public int PositionRank { get; init; } = 90;
	public string AddonName { get; init;} = "Use template with selection";
	public string AddonDescription { get; init;} = "";
	public string AddonFluentIconName { get; init; } =  "CalendarTemplate";
	public List<TfScreenRegionScope> Scopes { get; init; } = new ();

	[Parameter] 
	public TfSpaceViewSelectorActionScreenRegionContext RegionContext { get; set; } = default!;
	private async Task _clickHandler()
	{
		if (RegionContext.SelectedDataRows.Count == 0
		|| RegionContext.SpaceData is null
		|| RegionContext.User is null) return;

		var context = new TfUseTemplateContext
		{
			SelectedRowIds = RegionContext.SelectedDataRows,
			SpaceData = RegionContext.SpaceData,
			User = RegionContext.User
		};
		var dialog = await DialogService.ShowDialogAsync<TucUseTemplateDialog>(
				context,
				new DialogParameters()
				{
					PreventDismissOnOverlayClick = true,
					PreventScroll = true,
					Width = TfConstants.DialogWidthExtraLarge,
					TrapFocus = false
				});
	}		
}