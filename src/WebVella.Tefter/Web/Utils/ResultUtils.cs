using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.Web.Utils;

internal static class ResultUtils
{
	internal static void ProcessServiceResult(
		Result<object> result,
		string toastErrorMessage,
		string notificationErrorTitle,
		IToastService toastService,
		IMessageService messageService)
	{
		if (result.IsSuccess) return;
		var generalErrors = new List<string>();
		foreach (IError iError in result.Errors)
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
			SendErrorsToNotifications(notificationErrorTitle, generalErrors, null, messageService);
		}
	}

	internal static void SendErrorsToNotifications(
		string message,
		List<string> errors,
		string stackTrace,
		IMessageService messageService)
	{
		var divHtml = "<ul class='notification-list'>";
		foreach (var error in errors)
		{
			divHtml += $"<li>{error}</li>";
		}
		divHtml += "</ul>";

		if (stackTrace is not null)
		{
			divHtml += "<textarea style='width:100%;' readonly rows='4'>";
			divHtml += stackTrace;//.Replace(Environment.NewLine,"<br/>");
			divHtml += "</textarea>";
		}

		messageService.ShowMessageBar(options =>
		{
			options.Intent = MessageIntent.Error;
			options.Title = message;
			options.Body = divHtml;
			options.Timestamp = DateTime.Now;
			options.Timeout = null;
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
		SendErrorsToNotifications(notificationErrorTitle, new List<string> { exception.Message }, exception.StackTrace, messageService);

		return errorMessage;
	}

	internal static void ProcessFormSubmitResult(
		Result<object> result,
		string toastErrorMessage,
		string notificationErrorTitle,
		EditContext editContext,
		ValidationMessageStore messageStore,
		IToastService toastService,
		IMessageService messageService
		)
	{

		var generalErrors = new List<string>();
		var validationErrors = new List<ValidationError>();
		if (result is null || editContext is null || messageStore is null) return;
		if (result.IsSuccess) return;

		foreach (IError iError in result.Errors)
		{
			ProssessFormResultError(iError, validationErrors, generalErrors);
		}

		foreach (var valError in validationErrors)
		{
			var field = editContext.Field(valError.PropertyName);
			messageStore.Add(field, valError.Reason);
		}

		editContext.NotifyValidationStateChanged();
		if (generalErrors.Count > 0 && validationErrors.Count == 0)
		{
			toastService.ShowToast(ToastIntent.Error, toastErrorMessage);
			SendErrorsToNotifications(notificationErrorTitle, generalErrors, null, messageService);
		}
	}

	internal static void ProssessFormResultError(
		IError iError,
		List<ValidationError> validationErrors,
		List<string> generalErrors)
	{
		if (iError is null) return;

		if (iError is ValidationError)
		{
			var error = (ValidationError)iError;
			if (String.IsNullOrWhiteSpace(error.PropertyName))
				generalErrors.Add(error.Reason);
			else
			{
				validationErrors.Add(error);
			}
		}
		else
		{
			var error = (IError)iError;
			generalErrors.Add(error.Message);
		}

		if (iError.Reasons is not null)
		{
			foreach (IError iReason in iError.Reasons)
			{
				ProssessFormResultError(iReason, validationErrors, generalErrors);
			}
		}

	}

	internal static Exception ConvertResultToException(Result<object> result, string message = null)
	{
		if (String.IsNullOrWhiteSpace(message))
		{
			message = "System error";
			if (result.Errors.Count > 0)
			{
				message = result.Errors[0].Message;
			}
		}
		var ex = new Exception(message);
		foreach (IError error in result.Errors)
		{
			if (error is null) continue;
			if (error is ValidationError)
			{
				var errorObj = (ValidationError)error;
				if (!String.IsNullOrWhiteSpace(errorObj.PropertyName))
					ex.Data.Add(errorObj.PropertyName,errorObj.Reason);
				else
				{
					ex.Data.Add("",errorObj.Reason);
				}
			}
			else
			{
				ex.Data.Add("",error.Message);
			}
		
		}

		return ex;
	}
}
