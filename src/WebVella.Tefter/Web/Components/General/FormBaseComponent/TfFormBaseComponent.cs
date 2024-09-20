using Microsoft.AspNetCore.Components.Forms;
using WebVella.Tefter.Errors;

namespace WebVella.Tefter.Web.Components;

public class TfFormBaseComponent : TfBaseComponent
{
	/// <summary>
	/// Edit context for the derivative forms
	/// </summary>
	protected EditContext EditContext;

	/// <summary>
	/// Message store for adding server response validation errors to edit context
	/// </summary>
	protected ValidationMessageStore MessageStore;

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			if (EditContext is not null)
			{
				//EditContext.OnValidationRequested -= EditContext_OnValidationRequested;
				EditContext.OnFieldChanged -= EditContext_OnFieldChanged;
			}
		}
		return base.DisposeAsyncCore(disposing);
	}

	/// <summary>
	/// Must be called always in the Initialize of the derivative components to init the Edit context
	/// </summary>
	/// <param name="form"></param>
	protected void InitForm(object form)
	{
		EditContext = new EditContext(form);
		MessageStore = new ValidationMessageStore(EditContext);
		//EditContext.OnValidationRequested += EditContext_OnValidationRequested;
		EditContext.OnFieldChanged += EditContext_OnFieldChanged;
	}

	/// <summary>
	/// Clear the message store for the form to be able to submit
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	//protected void EditContext_OnValidationRequested(object sender, ValidationRequestedEventArgs e)
	//{
	//	//MessageStore.Clear();
	//}

	/// <summary>
	/// Clear the message store for the form to be able to submit
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void EditContext_OnFieldChanged(object sender, FieldChangedEventArgs e)
	{
		MessageStore.Clear(e.FieldIdentifier);
	}

	/// <summary>
	/// Process Result<object> from service calls and injects the errors in EditContext MessageStore
	/// </summary>
	/// <param name="result"></param>
	protected void ProcessFormSubmitResponse(Result<object> result)
	{
		ResultUtils.ProcessFormSubmitResult(
			result: result,
			toastErrorMessage: LOC("Unexpected Error! Check Notifications for details"),
			notificationErrorTitle: LOC("Unexpected Error!"),
			editContext: EditContext,
			messageStore: MessageStore,
			toastService: ToastService,
			messageService: MessageService
		);
	}
}
