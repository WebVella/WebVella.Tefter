using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.EmailSender.Components;
[LocalizationResource("WebVella.Tefter.EmailSender.Components.SendTestEmailDialog.SendTestEmailDialog", "WebVella.Tefter.EmailSender")]
public partial class SendTestEmailDialog : TfFormBaseComponent, IDialogContentComponent<EmailMessage>
{
	[Inject] public IState<TfAuxDataState> TfAuxDataState { get; set; }
	[Inject] public IEmailService EmailService { get; set; }
	[Parameter] public EmailMessage Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private SendTestEmailDialogFrom _form = new();
	private string _activeTab = "text";
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_form.Recipient = Content.Recipients.Count > 0 ? Content.Recipients[0].Address : null;
		_form.Subject = Content.Subject;
		_form.ContentText = Content.ContentText;
		_form.ContentHtml = Content.ContentHtml;
		base.InitForm(_form);
	}
	private async Task _sendEmail()
	{
		if (_isSubmitting) return;
		try
		{
			//Workaround to wait for the form to be bound 
			//on enter click without blur
			await Task.Delay(10);

			MessageStore.Clear();

			if (!EditContext.Validate()) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);
			var submit = new CreateEmailMessageModel
			{
				Subject = _form.Subject,
				TextBody = _form.ContentText,
				HtmlBody = _form.ContentHtml,
				Recipients = new List<EmailAddress> { new EmailAddress { Address = _form.Recipient } }
			};
			var result = EmailService.CreateEmailMessage(submit);

			ProcessFormSubmitResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("Message scheduled for sending"));
				await Dialog.CloseAsync(result.Value);
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isSubmitting = false;
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

}

public record SendTestEmailDialogFrom
{
	[Required]
	[DataType(DataType.EmailAddress)]
	[EmailAddress]
	public string Recipient { get; set; }
	[Required]
	public string Subject { get; set; }
	public string ContentHtml { get; set; }
	public string ContentText { get; set; }
}
