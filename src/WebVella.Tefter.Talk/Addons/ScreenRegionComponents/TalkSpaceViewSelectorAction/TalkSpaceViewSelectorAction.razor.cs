﻿//namespace WebVella.Tefter.Talk.Components;

//public partial class TalkSpaceViewSelectorAction : TfBaseComponent, 
//	ITfScreenRegionComponent<TfSpaceViewSelectorActionScreenRegionContext>
//{
//	public const string ID = "942d6fb0-4662-4c5c-ae52-123dd40375ac";
//	public const string NAME = "Add Talk Discussion to Selection";
//	public const string DESCRIPTION = "";
//	public const string FLUENT_ICON_NAME = "CommentMultiple";
//	public const int POSITION_RANK = 100;
//	public Guid AddonId { get; init; } = new Guid(ID);
//	public string AddonName { get; init; } = NAME;
//	public string AddonDescription { get; init; } = DESCRIPTION;
//	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
//	public int PositionRank { get; init; } = POSITION_RANK;
//	public List<TfScreenRegionScope> Scopes { get; init; } = new ();
//	[Parameter] 
//	public TfSpaceViewSelectorActionScreenRegionContext RegionContext { get; init; }

//	[Inject] protected IState<TfAppState> TfAppState { get; set; }

//	private IDialogReference _dialog;
//	private async Task _onClick()
//	{
//		var context = new TalkThreadModalContext
//		{
//			DataProviderId = TfAppState.Value.SpaceViewData.QueryInfo.DataProviderId,
//			SelectedRowIds = TfAppState.Value.SelectedDataRows,
//		};
//		_dialog = await DialogService.ShowDialogAsync<TalkThreadModal>(
//				context,
//				new DialogParameters()
//				{
//					PreventDismissOnOverlayClick = true,
//					PreventScroll = true,
//					Width = TfConstants.DialogWidthLarge,
//			TrapFocus = false
//				});
//	}
//}