using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Components.Forms;
using System;

namespace WebVella.Tefter.Utility;

internal static class ResultUtils
{
	internal static void ProcessServiceException(
		Exception exception,
		string toastErrorMessage,
		string toastValidationMessage,
		string notificationErrorTitle,
		IToastService toastService,
		IMessageService messageService)
	{
		if (exception is null)
			return;

		var generalErrors = new List<string>();
		var validationErrors = new List<string>();

		if (exception is TfValidationException)
		{
			var valEx = (TfValidationException)exception;

			if (!string.IsNullOrWhiteSpace(valEx.Message))
				generalErrors.Add(valEx.Message);

			//used GetDataAsValidationErrorList which combine all errors in one list and no 2 cycles needed
			var validationErrorList = valEx.GetDataAsValidationErrorList();
			foreach (var valError in validationErrorList)
			{
				if (String.IsNullOrWhiteSpace(valError.PropertyName))
					validationErrors.Add(valError.Message);
				else
					validationErrors.Add($"{valError.PropertyName}: {valError.Message}");
			}

			//using GetDataAsUsableDictionary variant
			//var data = valEx.GetDataAsUsableDictionary();
			//foreach (var propertyName in data.Keys)
			//{
			//	var errors = data[propertyName];
			//	foreach (var valError in errors)
			//	{
			//		if (String.IsNullOrWhiteSpace(propertyName))
			//			validationErrors.Add(valError.Message);
			//		else
			//			validationErrors.Add($"{propertyName}: {valError.Message}");
			//	}
			//}
		}
		else if (exception.GetType().IsAssignableFrom(typeof(TfException)))
		{
			var valEx = (TfException)exception;

			if (!string.IsNullOrWhiteSpace(valEx.Message))
				generalErrors.Add(valEx.Message);

			//used GetDataAsValidationErrorList which combine all errors in one list and no 2 cycles needed
			var validationErrorList = valEx.GetDataAsValidationErrorList();
			foreach (var valError in validationErrorList)
			{
				if (String.IsNullOrWhiteSpace(valError.PropertyName))
					generalErrors.Add(valError.Message);
				else
					generalErrors.Add($"{valError.PropertyName}: {valError.Message}");
			}

			//using GetDataAsUsableDictionary variant
			//var data = valEx.GetDataAsUsableDictionary();
			//foreach (var propertyName in data.Keys)
			//{
			//	var errors = data[propertyName];
			//	foreach (var valError in errors)
			//	{
			//		if (String.IsNullOrWhiteSpace(valError.PropertyName))
			//			generalErrors.Add(valError.Message);
			//		else
			//			generalErrors.Add($"{valError.PropertyName}: {valError.Message}");
			//	}
			//}
		}
		else
		{
			GetExceptionMessages(exception, generalErrors);
		}

		if (generalErrors.Count > 0)
		{
			toastService.ShowToast(ToastIntent.Error, toastErrorMessage);
			SendErrorsToNotifications(notificationErrorTitle, generalErrors, null, messageService);
		}
		else if (validationErrors.Count > 0)
		{
			toastService.ShowCommunicationToast(new ToastParameters<CommunicationToastContent>()
			{
				Intent = ToastIntent.Warning,
				Title = toastValidationMessage,
				Content = new CommunicationToastContent
				{
					Details = String.Join(Environment.NewLine, validationErrors)
				}
			});
		}
	}

	internal static void GetExceptionMessages(Exception ex, List<string> messages)
	{
		if (ex is null) return;
		if (!String.IsNullOrWhiteSpace(ex.Message))
		{
			messages.Add(ex.Message);
		}
		GetExceptionMessages(ex.InnerException, messages);
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
		var errors = new List<string> { };
		if (!String.IsNullOrWhiteSpace(exception.Message))
			errors.Add(exception.Message);
		if (exception.InnerException is not null)
		{
			errors.Add(exception.InnerException.Message);
		}
		errors.AddRange(TfConverters.GetDataAsErrorList(exception));
		SendErrorsToNotifications(notificationErrorTitle, errors, exception.StackTrace, messageService);

		return errorMessage;
	}

	internal static void ProcessFormSubmitException(
		Exception exception,
		string toastErrorMessage,
		string notificationErrorTitle,
		EditContext editContext,
		ValidationMessageStore messageStore,
		IToastService toastService,
		IMessageService messageService)
	{
		var generalErrors = new List<string>();
		var validationErrors = new List<ValidationError>();
		if (exception is null || editContext is null || messageStore is null) return;


		if (exception is TfValidationException)
		{
			var valEx = (TfValidationException)exception;

			if (!string.IsNullOrWhiteSpace(valEx.Message))
				generalErrors.Add(valEx.Message);

			//used GetDataAsValidationErrorList which combine all errors in one list and no 2 cycles needed
			var validationErrorList = valEx.GetDataAsValidationErrorList();
			validationErrors.AddRange(validationErrorList);


			//using GetDataAsUsableDictionary variant
			//var data = valEx.GetDataAsUsableDictionary();
			//foreach (var propertyName in data.Keys)
			//{
			//	var errors = data[propertyName];
			//	foreach (var valError in errors)
			//	{
			//		validationErrors.Add(valError);
			//	}
			//}
		}
		else if (exception.InnerException is not null)
		{
			generalErrors.Add(exception.InnerException.Message);
		}
		else
		{
			generalErrors.Add(exception.Message);
		}

		foreach (var valError in validationErrors)
		{
			var field = editContext.Field(valError.PropertyName);
			messageStore.Add(field, valError.Message);
		}

		editContext.NotifyValidationStateChanged();
		if (generalErrors.Count > 0 && validationErrors.Count == 0)
		{
			toastService.ShowToast(ToastIntent.Error, toastErrorMessage);
			SendErrorsToNotifications(notificationErrorTitle, generalErrors, null, messageService);
		}
	}
}
