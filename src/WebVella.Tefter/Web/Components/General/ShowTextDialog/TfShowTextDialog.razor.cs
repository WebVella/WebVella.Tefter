namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.ShowTextDialog.TfShowTextDialog", "WebVella.Tefter")]
public partial class TfShowTextDialog : TfFormBaseComponent, IDialogContentComponent<string>
{
	[Parameter] public string Content { get; set; }

	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}



}

