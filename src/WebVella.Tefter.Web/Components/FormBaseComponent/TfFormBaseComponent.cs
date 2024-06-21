using Microsoft.AspNetCore.Components.Forms;
using WebVella.Tefter.Errors;

namespace WebVella.Tefter.Web.Components.FormBaseComponent;

public class TfFromBaseComponent : TfBaseComponent
{
	/// <summary>
	/// Edit context for the derivative forms
	/// </summary>
	internal EditContext EditContext;

	/// <summary>
	/// Message store for adding server response validation errors to edit context
	/// </summary>
	internal ValidationMessageStore MessageStore;

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			EditContext.OnValidationRequested -= EditContext_OnValidationRequested;
		}
		return base.DisposeAsyncCore(disposing);
	}

	/// <summary>
	/// Must be called always in the Initialize of the derivative components to init the Edit context
	/// </summary>
	/// <param name="form"></param>
	internal void InitForm(object form)
	{
		EditContext = new EditContext(form);
		MessageStore = new ValidationMessageStore(EditContext);
		EditContext.OnValidationRequested += EditContext_OnValidationRequested;
	}

	/// <summary>
	/// Clear the message store for the form to be able to submit
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	internal void EditContext_OnValidationRequested(object sender, ValidationRequestedEventArgs e)
	{
		MessageStore.Clear();
	}

	/// <summary>
	/// Process Result<object> from service calls and injects the errors in EditContext MessageStore
	/// </summary>
	/// <param name="result"></param>
	internal void ProcessFormSubmitResponse(Result<object> result)
	{
		var generalErrors = new List<string>();
		if (result is null || EditContext is null || MessageStore is null) return;
		if (result.IsSuccess) return;

		foreach (IError iError in result.Errors)
		{
			if (iError is ValidationError)
			{
				var error = (ValidationError)iError;
				MessageStore.Add(EditContext.Field(error.PropertyName), error.Message);
			}
			else
			{
				var error = (IError)iError;
				generalErrors.Add(error.Message);
			}

		}
		EditContext.NotifyValidationStateChanged();
		if (generalErrors.Count > 0)
		{
			ToastService.ShowToast(ToastIntent.Error, "Unexpected Error! Check Notifications for details");
			var divHtml = "<ul class='notification-list'>";
			foreach (var error in generalErrors)
			{
				divHtml += $"<li>{error}</li>";
			}
			divHtml += "</ul>";

			MessageService.ShowMessageBar(options =>
			{
				options.Intent = MessageIntent.Error;
				options.Title = $"Unexpected Error!";
				options.Body = divHtml;
				options.Timestamp = DateTime.Now;
				options.Timeout = 15000;
				options.Section = TfConstants.MESSAGES_NOTIFICATION_CENTER;
			});
		}
	}
}
