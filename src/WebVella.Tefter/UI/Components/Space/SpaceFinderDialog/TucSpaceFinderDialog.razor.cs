namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceFinderDialog : TfBaseComponent, IDialogContentComponent<TfUser?>
{
	[Parameter] public TfUser? Content { get; set; }
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
