namespace WebVella.Tefter.Talk.Components;

public partial class TalkSpaceViewSelectorAction : TfBaseComponent, ITfRegionComponent<TfSpaceViewSelectorActionComponentContext>
{
	public Guid Id { get; init; } = new Guid("942d6fb0-4662-4c5c-ae52-123dd40375ac"); 
	public int PositionRank { get; init; } = 100;
	public string Name { get; init;} = "Add Talk Discussion to Selection";
	public string Description { get; init;} = "";
	public string FluentIconName { get; init; } =  "CommentMultiple";
	public List<TfRegionComponentScope> Scopes { get; init; } = new ();
	[Parameter] 
	public TfSpaceViewSelectorActionComponentContext Context { get; init; }

	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private IDialogReference _dialog;
	private async Task _onClick()
	{
		var context = new TalkThreadModalContext
		{
			DataProviderId = TfAppState.Value.SpaceViewData.QueryInfo.DataProviderId,
			SelectedRowIds = TfAppState.Value.SelectedDataRows,
		};
		_dialog = await DialogService.ShowDialogAsync<TalkThreadModal>(
				context,
				new DialogParameters()
				{
					PreventDismissOnOverlayClick = true,
					PreventScroll = true,
					Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
				});
	}
}