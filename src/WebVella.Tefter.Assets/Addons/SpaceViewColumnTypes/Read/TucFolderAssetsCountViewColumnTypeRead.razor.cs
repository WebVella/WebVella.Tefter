using WebVella.Tefter.MessagingEx;

namespace WebVella.Tefter.UI.Addons;

public partial class TucFolderAssetsCountViewColumnTypeRead : ComponentBase
{
	[Inject] protected ITfEventBusEx TfEventBus { get; set; } = null!;
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

                var change = ((AssetsFolderPanelContext)instance.Content).CountChange;
				if(change == 0) return;
                if (ColumnName is null) return;
                var changeRequest = new Dictionary<Guid, Dictionary<string, long>>
                {
	                [Context.RowId] = new()
	                {
		                [ColumnName] = change
	                }
                };
                var dataChange = Context.DataTable.ApplyCountChange(
                    countChange: changeRequest);
                if (dataChange is null) return;
                await TfEventBus.PublishAsync(
	                key: null,
	                payload: new TfSpaceViewDataUpdatedEventPayload(Context.ViewColumn.SpaceViewId,dataChange));	                
            })
		});
	}	
}
