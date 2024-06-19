using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.Web.Utils;

public static class ResponseUtils
{
	/// <summary>
	/// Fills in the editContext with validation errors returned from the server
	/// or displays Excelptions as toast message
	/// </summary>
	/// <param name="editContext"></param>
	/// <param name="ex"></param>
	public static void ProcessResponse(TfFromBaseComponent comp, Exception ex, IToastService toastService)
	{
		if (ex == null || comp is null) return;
		comp.MessageStore.Add(comp.EditContext.Field(nameof(TfLoginModel.Email)), ex.Message);
		comp.MessageStore.Add(comp.EditContext.Field(nameof(TfLoginModel.Email)), ex.Message);

		comp.EditContext.NotifyValidationStateChanged();

		toastService.ShowToast(ToastIntent.Error, ex.Message);
	}
}
