using WebVella.Tefter.UseCases.Login;

namespace WebVella.Tefter.Web.Components.Login;
public partial class TfLogin : TfFormBaseComponent
{
	internal LoginUseCase _useCase;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_useCase = new LoginUseCase(
			identityManager:IdentityManager
		).Initialize();

		base.InitForm(_useCase.Form);
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
			_useCase.IsSubmitting = true;
			await InvokeAsync(StateHasChanged);

			var result = await _useCase.AuthenticateAsync(JSRuntime);

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
			_useCase.IsSubmitting = false;
			await InvokeAsync(StateHasChanged);
		}
	}

}

