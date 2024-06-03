namespace WebVella.Tefter.Demo.Services;
using Microsoft.JSInterop;
public class LocalStorageService
{
	private readonly IJSRuntime _jsRuntime;

	public LocalStorageService(IJSRuntime jsRuntime)
	{
		_jsRuntime = jsRuntime;
	}

	public async Task AddItem(string key, string value)
	{
		await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
	}

	public async Task RemoveItem(string key)
	{
		await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
	}

	public async Task<string> GetItem(string key)
	{
		return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
	}
}
