namespace WebVella.Tefter.UI.Components;

public partial class TucLogin : TfFormBaseComponent
{
	[Inject] protected IJSRuntime JsRuntime { get; set; } = null!;
	[Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;	
	private bool _isLoading = true;
	private bool _isSubmitting = false;
	private TfLoginForm _form = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		InitForm(_form);
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			//Check if not already logged in
			var user = await TfService.GetUserFromCookieAsync(
				jsRuntime: JsRuntime,
				authStateProvider: AuthenticationStateProvider);

			if (user is not null)
			{
				Navigator.NavigateTo(TfConstants.HomePageUrl, true);
				return;
			}
		
			var installData = await TfService.GetInstallDataAsync();
			if (installData is null)
			{
				Navigator.NavigateTo(TfConstants.InstallPage, true);
				return;
			}

			_isLoading = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	internal async Task _submit()
	{
		try
		{
			//Workaround to wait for the form to be bound 
			//on enter click without blur
			await Task.Delay(10);
			MessageStore.Clear();
			if (!EditContext.Validate()) return;
			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);

			_ = await TfService.AuthenticateAsync(
				jsRuntime: JSRuntime,
				email: _form.Email,
				password: _form.Password,
				rememberMe: _form.RememberMe);

			Navigator.NavigateTo(TfConstants.HomePageUrl, true);
		}
		catch (Exception ex)
		{
			ProcessFormSubmitResponse(ex);
		}
		finally
		{
			_isSubmitting = false;
			await InvokeAsync(StateHasChanged);
		}
	}
}