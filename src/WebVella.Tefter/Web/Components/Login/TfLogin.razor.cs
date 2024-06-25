using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Web.Components.Login;
public partial class TfLogin : TfFormBaseComponent
{
	[Inject] internal IState<UserState> UserState { get; set; }

	internal TfLoginModel _form = new();
	internal bool _submitting = false;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		base.InitForm(_form);
	}

	internal async Task _submit()
	{
		try
		{
			//Workaround to wait for the form to be bound 
			//on enter click without blur
			await Task.Delay(10);
			var isValid = EditContext.Validate();
			if(!isValid) return;
			_submitting = true;
			await InvokeAsync(StateHasChanged);

			var result = await IdentityManager.AuthenticateAsync(
				JSRuntime, _form.Email, _form.Password, _form.RememberMe);

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
			_submitting = false;
			await InvokeAsync(StateHasChanged);
		}
	}

}

internal class TfLoginModel
{
	[Required(ErrorMessage = "email required")]
	[EmailAddress(ErrorMessage = "invalid email")]
	public string Email { get; set; }

	[Required(ErrorMessage = "password required")]
	public string Password { get; set; }
	public bool RememberMe { get; set; } = true;
}