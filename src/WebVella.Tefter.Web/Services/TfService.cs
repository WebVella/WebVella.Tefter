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
	private IDataBroker dataBroker;

	public TfService(ProtectedLocalStorage protectedLocalStorage, IDataBroker dataBroker)
	{
		this.browserStorage = protectedLocalStorage;
		this.dataBroker = dataBroker;
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
