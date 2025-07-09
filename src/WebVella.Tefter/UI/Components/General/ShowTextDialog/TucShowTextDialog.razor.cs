namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.UI.Components.General.ShowTextDialog.TucShowTextDialog", "WebVella.Tefter")]
public partial class TucShowTextDialog : TfFormBaseComponent, IDialogContentComponent<string>
{
	[Parameter] public string Content { get; set; }

	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}



}

