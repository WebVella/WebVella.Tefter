namespace WebVella.Tefter.Talk.Components;
[TfScreenRegionComponent(TfScreenRegion.SpaceViewSelectorActions, 10, null, null)]
public partial class TalkSpaceViewSelectorAction : TucBaseScreenRegionComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private IDialogReference? _dialog;
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
					Width = TfConstants.DialogWidthLarge
				});
	}
}