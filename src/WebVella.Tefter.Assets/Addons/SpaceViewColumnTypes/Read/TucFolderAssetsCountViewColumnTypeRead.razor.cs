using WebVella.Tefter.UI.Layout;

namespace WebVella.Tefter.UI.Addons;

public partial class TucFolderAssetsCountViewColumnTypeRead : ComponentBase
{
	[CascadingParameter(Name = "TfAuthLayout")] public TfAuthLayout TfAuthLayout { get; set; } = null!;
	[Inject] protected ITfEventBus TfEventBus { get; set; } = null!;
	[Inject] public IDialogService DialogService { get; set; } = null!;
	[Parameter] public List<long?>? Value { get; set; }
	[Parameter] public TfSpaceViewColumnReadMode Context { get; set; } = null!;
	[Parameter] public string? ColumnName { get; set; } = null!;
	
    private async Task _onClick()
	{
		if(Context.DataTable is null) return;
		var panelContext = new AssetsFolderPanelContext
		{
			FolderId = Context.GetSettings<TfFolderAssetsCountViewColumnTypeSettings>().FolderId,
			DataTable = Context.DataTable,
			RowId = Context.RowId
		};

		_ = await DialogService.ShowPanelAsync<AssetsFolderPanel>(
		panelContext,
		new ()
		{
			DialogType = DialogType.Panel,
			Alignment = HorizontalAlignment.Right,
			ShowTitle = false,
			ShowDismiss = false,
			PrimaryAction = null,
			SecondaryAction = null,
			Width = "25vw",
			TrapFocus = false,
			OnDialogClosing = EventCallback.Factory.Create<DialogInstance>(this, async (instance) =>
			{
                await TfEventBus.PublishAsync(
	                key: TfAuthLayout.GetSessionId(),
	                payload: new TfSpaceViewDataUpdatedEventPayload(Context.ViewColumn.SpaceViewId,[Context.RowId]));	                
            })
		});
	}	
}
