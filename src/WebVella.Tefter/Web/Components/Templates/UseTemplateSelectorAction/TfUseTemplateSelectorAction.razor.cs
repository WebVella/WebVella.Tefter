namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.UseTemplateSelectorAction.TfUseTemplateSelectorAction", "WebVella.Tefter")]

public partial class TfUseTemplateSelectorAction : TfBaseComponent, ITfScreenRegionComponent<TfSpaceViewSelectorActionScreenRegionContext>
{
	//Injects
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	//State
	public Guid AddonId { get; init; } = new Guid("3e344931-988c-4f84-874d-823e31ec73ad"); 
	public int PositionRank { get; init; } = 90;
	public string AddonName { get; init;} = "Use template with selection";
	public string AddonDescription { get; init;} = "";
	public string AddonFluentIconName { get; init; } =  "CalendarTemplate";
	public List<TfScreenRegionScope> Scopes { get; init; } = new ();

	[Parameter] 
	public TfSpaceViewSelectorActionScreenRegionContext RegionContext { get; init; }

	private async Task _clickHandler()
	{
		if (TfAppState.Value.SelectedDataRows.Count == 0
		|| TfAppState.Value.SpaceView.SpaceDataId is null) return;

		var context = new TucUseTemplateContext
		{
			SelectedRowIds = TfAppState.Value.SelectedDataRows,
			SpaceData = TfAppState.Value.SpaceData,
			User = TfAppState.Value.CurrentUser
		};
		var dialog = await DialogService.ShowDialogAsync<TfUseTemplateDialog>(
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