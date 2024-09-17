namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SearchSpaceDialog.TfSearchSpaceDialog","WebVella.Tefter")]
public partial class TfSearchSpaceDialog : TfFormBaseComponent, IDialogContentComponent<bool>
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Parameter] public bool Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

}
