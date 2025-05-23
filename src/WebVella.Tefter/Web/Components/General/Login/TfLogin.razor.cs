﻿using WebVella.Tefter.UseCases.Recipe;

namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.Login.TfLogin", "WebVella.Tefter")]
public partial class TfLogin : TfFormBaseComponent
{
	[Inject] private LoginUseCase UC { get; set; }
	[Inject] private RecipeUseCase RecipeUC { get; set; }
	[Inject] private AppStateUseCase AppUC { get; set; }
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		var installData = await RecipeUC.GetInstallDataAsync();
		if (installData is null)
			Navigator.NavigateTo(TfConstants.InstallPage, true);
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
			if (result)
				Navigator.NavigateTo(TfConstants.HomePageUrl, true);
		}
		catch (Exception ex)
		{
			ProcessFormSubmitResponse(ex);
		}
		finally
		{
			UC.IsSubmitting = false;
			await InvokeAsync(StateHasChanged);
		}
	}

}

