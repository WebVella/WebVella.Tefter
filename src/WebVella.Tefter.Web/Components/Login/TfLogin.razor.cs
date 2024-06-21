﻿
using WebVella.Tefter.Utility;

namespace WebVella.Tefter.Web.Components.Login;
public partial class TfLogin : TfFromBaseComponent
{
    [Inject] internal ICryptoService CryptoService { get; set; }
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
            _submitting = true;
            await InvokeAsync(StateHasChanged);
            Result<User> result = await IdentityManager.GetUserAsync(_form.Email, _form.Password);
            ProcessFormSubmitResponse(result);
            if (result.IsSuccess)
            {
                //Set auth cookie
                await new CookieService(JSRuntimeSrv).SetAsync(
                        key:Constants.TEFTER_AUTH_COOKIE_NAME,
                        value: CryptoService.Encrypt(result.Value.Id.ToString()),
                        expiration:_form.RememberMe ? DateTimeOffset.Now.AddDays(30) : null);
                Navigator.NavigateTo(TfConstants.HomePageUrl,true);
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
    public bool RememberMe { get; set; } = true;
}