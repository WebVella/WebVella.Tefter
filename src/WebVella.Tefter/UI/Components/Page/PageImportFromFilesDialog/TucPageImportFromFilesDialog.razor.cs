namespace WebVella.Tefter.UI.Components;
public partial class TucPageImportFromFilesDialog : TfBaseComponent, IDialogContentComponent<List<FluentInputFileEventArgs>?>
{
	[Parameter] public List<FluentInputFileEventArgs>? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;

	
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
	}
	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

}
