using WebVella.Tefter.UI.Components;

namespace WebVella.Tefter.Talk.Components;

public partial class TalkSpaceViewSelectorAction : TfBaseComponent,
	ITfScreenRegionAddon<TfSpaceViewSelectorActionScreenRegion>
{
	public const string ID = "942d6fb0-4662-4c5c-ae52-123dd40375ac";
	public const string NAME = "Add Talk Discussion to Selection";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "CommentMultiple";
	public const int POSITION_RANK = 100;
	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new();
	[Parameter]
	public TfSpaceViewSelectorActionScreenRegion RegionContext { get; set; }

	[CascadingParameter(Name = "TucSpaceViewPageContent")]
	public TucSpaceViewPageContent TucSpaceViewPageContent { get; set; } = null!;

	private IDialogReference _dialog;
	private async Task _onClick()
	{
		var context = new TalkThreadModalContext
		{
			DataProviderId = RegionContext.SpaceData.DataProviderId,
			SelectedRowIds = RegionContext.SelectedDataRows,
			CurrentUser = RegionContext.CurrentUser
		};
		_dialog = await DialogService.ShowDialogAsync<TalkThreadModal>(
				context,
				new ()
				{
					PreventDismissOnOverlayClick = true,
					PreventScroll = true,
					Width = TfConstants.DialogWidthLarge,
					TrapFocus = false,
					OnDialogClosing = EventCallback.Factory.Create<DialogInstance>(this, async (instance) =>
					{
      //                  var dataChange = TucSpaceViewPageContent.GetCurrentData().ApplyCountChange(
						//	countChange: ((TalkThreadModalContext)instance.Content).CountChange);
						//if (dataChange is null) return;
						//TucSpaceViewPageContent.OnDataChange(dataChange);
					})
				});
	}

}