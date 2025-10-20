namespace WebVella.Tefter.UI.Addons;

public partial class TucTalkCommentsCountViewColumnTypeRead : ComponentBase
{
	[Inject] public IDialogService DialogService { get; set; } = null!;
	[Parameter] public List<long?>? Value { get; set; }
	[Parameter] public TfSpaceViewColumnReadModeContext Context { get; set; } = null!;
	[Parameter] public string? ColumnName { get; set; } = null!;
	
    private async Task _onClick()
	{
		var panelContext = new TalkThreadPanelContext
		{
			ChannelId = Context.GetSettings<TfTalkCommentsCountViewColumnTypeSettings>().ChannelId,
			DataTable = Context.DataTable,
			RowId = Context.RowId
		};

		_ = await DialogService.ShowPanelAsync<TalkThreadPanel>(
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

                var change = ((TalkThreadPanelContext)instance.Content).CountChange;
				if(change == 0) return;
                if (ColumnName is null) return;
                var changeRequest = new Dictionary<Guid, Dictionary<string, long>>();
                changeRequest[Context.RowId] = new();
                changeRequest[Context.RowId][ColumnName] = change;
                var dataChange = Context.DataTable.ApplyCountChange(
                    countChange: changeRequest);
                if (dataChange is null) return;
                await Context.TfService.PublishEventWithScopeAsync(new TfSpaceViewDataChangedEvent(Context.ViewColumn.SpaceViewId,dataChange));
            })
		});
	}	
}
