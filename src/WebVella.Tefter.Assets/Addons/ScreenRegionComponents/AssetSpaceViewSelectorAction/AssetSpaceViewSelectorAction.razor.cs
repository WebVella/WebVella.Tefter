using WebVella.Tefter.UI.Components;

namespace WebVella.Tefter.Talk.Components;

public partial class AssetSpaceViewSelectorAction : TfBaseComponent,
	ITfScreenRegionAddon<TfSpaceViewSelectorActionScreenRegion>
{
	public const string ID = "c899bbe1-eade-4a00-a16e-6af87348ac71";
	public const string NAME = "Add Assets to Selection";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "DocumentDataLink";
	public const int POSITION_RANK = 200;
	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new();
	[Parameter]
	public TfSpaceViewSelectorActionScreenRegion RegionContext { get; set; }

	private IDialogReference _dialog;
	private async Task _onClick()
	{
		var context = new AssetsAttachModalContext
		{
			DataProviderId = RegionContext.Dataset.DataProviderId,
			SelectedRowIds = RegionContext.SelectedDataRows,
			CurrentUser = RegionContext.CurrentUser,
			SpaceViewId = RegionContext.SpaceView.Id
		};
		_dialog = await DialogService.ShowDialogAsync<AssetsAttachModal>(
				context,
				new ()
				{
					PreventDismissOnOverlayClick = true,
					PreventScroll = true,
					Width = TfConstants.DialogWidthLarge,
					TrapFocus = false,
				});
	}
	
}