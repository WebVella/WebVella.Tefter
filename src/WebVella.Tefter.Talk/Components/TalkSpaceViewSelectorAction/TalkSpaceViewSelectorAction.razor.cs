namespace WebVella.Tefter.Talk.Components;

public partial class TalkSpaceViewSelectorAction : TucBaseScreenRegionComponent, ITfScreenRegionComponent
{
	public Guid Id { get { return new Guid("dbf92a2b-f235-4d88-b982-b7a8be17c2b1"); } }
	public TfScreenRegion ScreenRegion { get { return TfScreenRegion.SpaceViewSelectorActions; } }
	public int Position { get { return 10; } }
	public string Name { get { return null; } }
	public string FluentIconName => "CommentMultiple";

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