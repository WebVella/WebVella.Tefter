namespace WebVella.Tefter.UI.Components;
public partial class TucShowTextDialog : TfFormBaseComponent, IDialogContentComponent<string>
{
	[Parameter] public string Content { get; set; }

	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}



}

