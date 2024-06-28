using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace WebVella.Tefter.Web.Services;
public partial interface ITfService{
	Task SetUnprotectedLocalStorage(string key, string value);
	Task RemoveUnprotectedLocalStorage(string key);
	Task<string> GetUnprotectedLocalStorage(string key);
}

public partial class TfService : ITfService
{
	//private ProtectedLocalStorage browserStorage;
	private IDataBroker dataBroker;
	private IIdentityManager identityManager;
	private ITfDataProviderManager dataProviderManager;
	private readonly IJSRuntime _jsRuntime;

	public TfService(IDataBroker dataBroker,
		IIdentityManager identityManager,
		ITfDataProviderManager dataProviderManager,
		IJSRuntime jsRuntime)
	{
		//this.browserStorage = protectedLocalStorage;
		this.dataBroker = dataBroker;
		this.identityManager = identityManager;
		this.dataProviderManager = dataProviderManager;
		_jsRuntime = jsRuntime;
	}

	public async Task SetUnprotectedLocalStorage(string key, string value)
	{
		await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
	}

	public async Task RemoveUnprotectedLocalStorage(string key)
	{
		await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
	}

	public async Task<string> GetUnprotectedLocalStorage(string key)
	{
		return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
	}
}
