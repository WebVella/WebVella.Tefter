using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.Web.Utils;

internal static class ResultUtils
{
	internal static void ProcessServiceResponse(
		Result<object> response,
		string toastErrorMessage,
		string notificationErrorTitle,
		IToastService toastService,
		IMessageService messageService)
	{
		if (response.IsSuccess) return;
		var generalErrors = new List<string>();
		foreach (IError iError in response.Errors)
		{
			if (iError is ValidationError)
			{
				var error = (ValidationError)iError;
				if (String.IsNullOrWhiteSpace(error.PropertyName))
					generalErrors.Add(error.Reason);
			}
			else
			{
				var error = (IError)iError;
				generalErrors.Add(error.Message);
			}

		}
		if (generalErrors.Count > 0)
		{
			toastService.ShowToast(ToastIntent.Error, toastErrorMessage);
			SendErrorsToNotifications(notificationErrorTitle, generalErrors,messageService);
		}
	}

	internal static void SendErrorsToNotifications(
		string message, 
		List<string> errors,
		IMessageService messageService)
	{
		var divHtml = "<ul class='notification-list'>";
		foreach (var error in errors)
		{
			divHtml += $"<li>{error}</li>";
		}
		divHtml += "</ul>";

		messageService.ShowMessageBar(options =>
		{
			options.Intent = MessageIntent.Error;
			options.Title = message;
			options.Body = divHtml;
			options.Timestamp = DateTime.Now;
			options.Timeout = 15000;
			options.AllowDismiss = true;
			options.Section = TfConstants.MESSAGES_NOTIFICATION_CENTER;
		});
	}

	internal static string ProcessException(
		Exception exception,
		string toastErrorMessage,
		string notificationErrorTitle,
		IToastService toastService,
		IMessageService messageService
		)
	{
		string errorMessage = toastErrorMessage;
		toastService.ShowToast(ToastIntent.Error, errorMessage);
		SendErrorsToNotifications(notificationErrorTitle, new List<string> { exception.Message },messageService);

		return errorMessage;
	}

	internal static void ProcessFormSubmitResponse(
		Result<object> result,
		string toastErrorMessage,
		string notificationErrorTitle,
		EditContext editContext,
		ValidationMessageStore messageStore,
		IToastService toastService,
		IMessageService messageService		
		){
		
	var generalErrors = new List<string>();
		if (result is null || editContext is null || messageStore is null) return;
		if (result.IsSuccess) return;

		foreach (IError iError in result.Errors)
		{
			if (iError is ValidationError)
			{
				var error = (ValidationError)iError;
				if (String.IsNullOrWhiteSpace(error.PropertyName))
					generalErrors.Add(error.Reason);
				else
					messageStore.Add(editContext.Field(error.PropertyName), error.Reason);
			}
			else
			{
				var error = (IError)iError;
				generalErrors.Add(error.Message);
			}

		}
		editContext.NotifyValidationStateChanged();
		if (generalErrors.Count > 0)
		{
			toastService.ShowToast(ToastIntent.Error, toastErrorMessage);
			SendErrorsToNotifications(notificationErrorTitle, generalErrors,messageService);
		}	
		}
}
