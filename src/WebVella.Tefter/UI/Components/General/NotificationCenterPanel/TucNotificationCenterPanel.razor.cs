namespace WebVella.Tefter.UI.Components;
public partial class TucNotificationCenterPanel : TfBaseComponent, IDialogContentComponent<bool>,IDisposable
{
	[Parameter]
	public bool Content { get; set; } = true;

	[CascadingParameter] public FluentDialog Dialog { get; set; }

	//private IDialogReference? _dialog;
	public void Dispose()
	{
		MessageService.OnMessageItemsUpdated -= UpdateCount;
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

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

	private async Task _dismissAll()
	{
		MessageService.Clear(TfConstants.MESSAGES_NOTIFICATION_CENTER);
		await _cancel();
	}

}