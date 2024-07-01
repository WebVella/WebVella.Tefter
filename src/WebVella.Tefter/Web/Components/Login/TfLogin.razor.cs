using WebVella.Tefter.UseCases.Login;

namespace WebVella.Tefter.Web.Components.Login;
public partial class TfLogin : TfFormBaseComponent
{
	[Inject] private LoginUseCase UC { get; set; }
	protected override void OnInitialized()
	{
		base.OnInitialized();
		UC.OnInitialized();
		base.InitForm(UC.Form);
	}

	internal async Task _submit()
	{
		try
		{
			//Workaround to wait for the form to be bound 
			//on enter click without blur
			await Task.Delay(10);
			var isValid = EditContext.Validate();
			if (!isValid) return;
			UC.IsSubmitting = true;
			await InvokeAsync(StateHasChanged);

			var result = await UC.AuthenticateAsync(JSRuntime);

			ProcessFormSubmitResponse(result);
			if (result.IsSuccess)
				Navigator.NavigateTo(TfConstants.HomePageUrl, true);
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			UC.IsSubmitting = false;
			await InvokeAsync(StateHasChanged);
		}
	}

}

