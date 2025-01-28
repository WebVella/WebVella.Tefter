namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.UseTemplateSelectorAction.TfUseTemplateSelectorAction", "WebVella.Tefter")]

public partial class TfUseTemplateSelectorAction : TfBaseComponent, ITfRegionComponent<TfSpaceViewSelectorActionComponentContext>
{
	//Injects
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	//State
	public Guid Id { get; init; } = new Guid("3e344931-988c-4f84-874d-823e31ec73ad"); 
	public int PositionRank { get; init; } = 90;
	public string Name { get; init;} = "Use template with selection";
	public string Description { get; init;} = "";
	public string FluentIconName { get; init; } =  "CalendarTemplate";
	public List<TfRegionComponentScope> Scopes { get; init; } = new ();

	[Parameter] 
	public TfSpaceViewSelectorActionComponentContext Context { get; init; }

	private IDialogReference _dialog;
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