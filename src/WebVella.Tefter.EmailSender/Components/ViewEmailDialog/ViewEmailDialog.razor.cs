using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.EmailSender.Components;
[LocalizationResource("WebVella.Tefter.EmailSender.Components.ViewEmailDialog.ViewEmailDialog", "WebVella.Tefter.EmailSender")]
public partial class ViewEmailDialog : TfBaseComponent, IDialogContentComponent<EmailMessage>
{
	[Inject] public IEmailService TalkService { get; set; }
	[Parameter] public EmailMessage Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private string _activeTab = "text";

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

}
