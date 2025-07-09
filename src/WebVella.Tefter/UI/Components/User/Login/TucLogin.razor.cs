namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.UI.Components.General.Login.TucLogin", "WebVella.Tefter")]
public partial class TucLogin : TfFormBaseComponent
{
	[Inject] private ITfUserUIService TfUserUIService { get; set; } = default!;
	[Inject] private ITfRecipeUIService TfRecipeUIService { get; set; } = default!;

	private bool _isSubmitting = false;
	private TfLoginForm _form = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		var installData = await TfRecipeUIService.GetInstallDataAsync();
		if (installData is null)
			Navigator.NavigateTo(TfConstants.InstallPage, true);
		base.InitForm(_form);
	}

	internal async Task _submit()
	{
		try
		{
			//Workaround to wait for the form to be bound 
			//on enter click without blur
			//await Task.Delay(10);
			var isValid = EditContext.Validate();
			if (!isValid) return;
			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);

			var result = await TfUserUIService.AuthenticateAsync(_form);
			if (result is not null)
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

