using Microsoft.JSInterop;

namespace WebVella.Tefter.EmailSender.Components;
[LocalizationResource("WebVella.Tefter.EmailSender.Components.EmailSenderLogAdmin.EmailSenderLogAdmin", "WebVella.Tefter.EmailSender")]
public partial class EmailSenderLogAdmin : TfBaseComponent
{
	[Inject] public IEmailService EmailService { get; set; }
	[Inject] protected IState<TfAuxDataState> TfAuxDataState { get; set; }


	private async Task _viewEmailHandler(EmailMessage message)
	{
		var dialog = await DialogService.ShowDialogAsync<ViewEmailDialog>(
		message,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			

		}
	}

	private async Task _resentEmailHandler(EmailMessage message)
	{
		//try
		//{
		//	var result = EmailService.CreateEmailMessage

		//	ProcessFormSubmitResponse(result);
		//	if (result.IsSuccess)
		//	{
		//		ToastService.ShowSuccess(LOC("Message scheduled for sending"));
		//		await Dialog.CloseAsync(result.Value);
		//	}
		//}
		//catch (Exception ex)
		//{
		//	ProcessException(ex);
		//}
		//finally
		//{
		//	_isSubmitting = false;
		//	await InvokeAsync(StateHasChanged);
		//}
	}

	private async Task _cancelEmailHandler(EmailMessage message)
	{
		
	}
}