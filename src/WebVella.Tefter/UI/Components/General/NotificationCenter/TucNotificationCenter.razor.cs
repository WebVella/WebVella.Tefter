namespace WebVella.Tefter.UI.Components;
public partial class TucNotificationCenter : TfBaseComponent, IDisposable
{
	private IDialogReference _dialog;
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

	private async Task OpenNotificationCenterAsync()
	{
		_dialog = await DialogService.ShowPanelAsync<TucNotificationCenterPanel>(
		true,
		new ()
		{
			DialogType = DialogType.Panel,
			Alignment = HorizontalAlignment.Right,
			ShowTitle = false,
			ShowDismiss = false,
			PrimaryAction = null,
			SecondaryAction = null,
			Width = "25vw",
			PreventDismissOnOverlayClick = false,
			TrapFocus = false
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