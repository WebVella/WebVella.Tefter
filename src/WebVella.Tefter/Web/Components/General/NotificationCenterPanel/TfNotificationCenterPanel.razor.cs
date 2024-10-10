namespace WebVella.Tefter.Web.Components;
public partial class TfNotificationCenterPanel : TfBaseComponent, IDialogContentComponent<GlobalState>
{
	[Parameter]
	public GlobalState Content { get; set; } = default!;

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