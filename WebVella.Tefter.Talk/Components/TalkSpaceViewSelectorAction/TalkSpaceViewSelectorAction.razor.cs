namespace WebVella.Tefter.Talk.Components;

public partial class TalkSpaceViewSelectorAction : TucBaseScreenRegionComponent, ITfScreenRegionComponent
{
	public TfScreenRegion ScreenRegion { get { return TfScreenRegion.SpaceViewSelectorActions; } }
	public int Position { get { return 10; } }
	public string Name { get { return null; } }
	public string UrlSlug { get { return null; } } 

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
					Width = TfConstants.DialogWidthLarge
				});
	}
}