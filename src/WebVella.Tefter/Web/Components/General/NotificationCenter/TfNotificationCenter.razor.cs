namespace WebVella.Tefter.Web.Components;
public partial class TfNotificationCenter : TfBaseComponent
{
	private IDialogReference _dialog;
	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
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

	private async Task OpenNotificationCenterAsync()
	{
		_dialog = await DialogService.ShowPanelAsync<TfNotificationCenterPanel>(new DialogParameters<GlobalState>()
		{
			Alignment = HorizontalAlignment.Right,
			Title = $"Notifications",
			PrimaryAction = null,
			SecondaryAction = null,
			ShowDismiss = true,
			Width = "25vw",
			PreventDismissOnOverlayClick = false
		});
		DialogResult result = await _dialog.Result;
		HandlePanel(result);
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