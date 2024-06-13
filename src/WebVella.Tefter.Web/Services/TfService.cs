using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace WebVella.Tefter.Web.Services;
public partial interface ITfService
{
    Task SetString();
    ValueTask<string?> GetString();
}

public partial class TfService : ITfService
{
	private ProtectedLocalStorage browserStorage;

	public TfService(ProtectedLocalStorage protectedLocalStorage)
	{
		browserStorage = protectedLocalStorage;
	}


	public async Task SetString(){
		await browserStorage.SetAsync(TfConstants.UISettingsLocalKey,"testing");
	}

	public async ValueTask<string?> GetString()
	{
		var result = await browserStorage.GetAsync<string>(TfConstants.UISettingsLocalKey);

		return result.Value;
	}
}
