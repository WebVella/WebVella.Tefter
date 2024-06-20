
using EnumExtensions = WebVella.Tefter.Web.Utils.EnumExtensions;

namespace WebVella.Tefter.Web.Components.Login;
public partial class TfLogin : TfFromBaseComponent
{
	[Inject] internal IState<UserState> UserState { get; set; }

	internal TfLoginModel _form = new();
	internal bool _submitting = false;
	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			UserState.StateChanged += UserState_StateChanged;
		}
		return base.DisposeAsyncCore(disposing);
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();
		base.InitForm(_form);
		UserState.StateChanged += UserState_StateChanged;
	}

	private void UserState_StateChanged(object sender, EventArgs e)
	{
		if (UserState.Value.Loading) return;
		if (UserState.Value.User is not null)
		{
			Navigator.NavigateTo("/");
		}
	}

	internal async Task _submit()
	{
		try
		{
			_submitting = true;
			await InvokeAsync(StateHasChanged);
			Result<User> result = await IdentityManager.GetUserAsync(_form.Email, _form.Password);
			ProcessFormSubmitResponse(result);
			if (result.Value is null) throw new Exception("user not found");
			if (result.IsSuccess)
			{
				//Temporary we need to set the userId in the localstorage
				// until the serverside cookie setting is ready
				await ProtectedLocalStorage.SetAsync(TfConstants.UserLocalKey, result.Value.Id);

				await TfSrv.SetUnprotectedLocalStorage(TfConstants.UIThemeLocalKey, JsonSerializer.Serialize(new ThemeSettings { 
					ThemeMode = result.Value.Settings.ThemeMode, 
					ThemeColor = result.Value.Settings.ThemeColor
				}));

				Dispatcher.Dispatch(new SetUserAction(result.Value));
			}
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
}