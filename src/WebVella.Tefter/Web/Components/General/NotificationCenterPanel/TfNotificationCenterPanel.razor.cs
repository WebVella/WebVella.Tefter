namespace WebVella.Tefter.Web.Components;
public partial class TfNotificationCenterPanel : TfBaseComponent, IDialogContentComponent<bool>
{
	[Parameter]
	public bool Content { get; set; } = true;

	[CascadingParameter] public FluentDialog Dialog { get; set; }

	//private IDialogReference? _dialog;
    protected override ValueTask DisposeAsyncCore(bool disposing)
    {
        if (disposing) {
            MessageService.OnMessageItemsUpdated -= UpdateCount;
        }
        return base.DisposeAsyncCore(disposing);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        MessageService.OnMessageItemsUpdated += UpdateCount;
    }

    private void UpdateCount()
    {
        InvokeAsync(StateHasChanged);
    }

    private Task OpenNotificationCenterAsync()
    {
        //_dialog = await DialogService.ShowPanelAsync<NotificationCenterPanel>(new DialogParameters<GlobalState>()
        //{
        //    Alignment = HorizontalAlignment.Right,
        //    Title = $"Notifications",
        //    PrimaryAction = null,
        //    SecondaryAction = null,
        //    ShowDismiss = true
        //});
        //DialogResult result = await _dialog.Result;
        //HandlePanel(result);
		return Task.CompletedTask;
    }
	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

	private async Task _dismissAll(){
		MessageService.Clear(TfConstants.MESSAGES_NOTIFICATION_CENTER);
		await _cancel();
	}
    private static void HandlePanel(DialogResult result)
    {
        if (result.Cancelled)
        {
            return;
        }

        if (result.Data is not null)
        {
            return;
        }
    }


}