using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.Web.Components;

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
	protected void InitForm(object form){
		EditContext = new EditContext(form);
		MessageStore = new ValidationMessageStore(EditContext);
		EditContext.OnValidationRequested += EditContext_OnValidationRequested;
	}

	internal void EditContext_OnValidationRequested(object sender, ValidationRequestedEventArgs e)
	{
		MessageStore.Clear();
	}
}
