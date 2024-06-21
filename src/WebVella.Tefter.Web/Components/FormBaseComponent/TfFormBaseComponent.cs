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
				if (String.IsNullOrWhiteSpace(error.PropertyName))
					generalErrors.Add(error.Reason);
				else
					MessageStore.Add(EditContext.Field(error.PropertyName), error.Reason);
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
			ToastService.ShowToast(ToastIntent.Error, LOC("Unexpected Error! Check Notifications for details"));
			SendErrorsToNotifications(LOC("Unexpected Error!"), generalErrors);
		}
	}
}
