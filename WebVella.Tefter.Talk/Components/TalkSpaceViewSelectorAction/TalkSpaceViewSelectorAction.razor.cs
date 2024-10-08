namespace WebVella.Tefter.Talk.Components;
[TfScreenRegionComponent(TfScreenRegion.SpaceViewSelectorActions, 10, null, null)]
public partial class TalkSpaceViewSelectorAction : TucBaseScreenRegionComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private IDialogReference? _dialog;
	private async Task _onClick()
	{
		_dialog = await DialogService.ShowDialogAsync<TalkThreadModal>(
				null,
				new DialogParameters()
				{
					PreventDismissOnOverlayClick = true,
					PreventScroll = true,
					Width = TfConstants.DialogWidthLarge
				});
	}
}