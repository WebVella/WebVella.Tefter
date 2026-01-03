namespace WebVella.Tefter.Utility;

public static partial class NavigatorExt
{
	internal static async Task ToErrorPage(this NavigationManager navigator,IJSRuntime jsRuntime, string errorMessage)
	{
		await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", TfConstants.ErrorPageLocalStorageKey, errorMessage);
		navigator.NavigateTo("/error",true);
	}	
	internal static async Task<string?> GetErrorMessage(this NavigationManager navigator,IJSRuntime jsRuntime)
	{
		return await jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", TfConstants.ErrorPageLocalStorageKey);
	}

}