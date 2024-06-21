namespace WebVella.Tefter.Web.Services;

public class Cookie
{
	public string Key { get; set; }
	public string Value { get; set; }
	public DateTimeOffset? Expiration { get; set; }

	public Cookie(string key, string value, DateTimeOffset? expiration = null)
	{
		Key = key;
		Value = value;
		Expiration = expiration;
	}
}

public class CookieService
{
    readonly IJSRuntime JSRuntime;

    public CookieService(IJSRuntime jsRuntime)
    {
        JSRuntime = jsRuntime;
    }

	public async Task<IEnumerable<Cookie>> GetAllAsync()
	{
		var raw = await JSRuntime.InvokeAsync<string>("eval", "document.cookie");
		if (string.IsNullOrWhiteSpace(raw)) return Enumerable.Empty<Cookie>();

		return raw.Split("; ").Select(x =>
		{
			var parts = x.Split("=");
			if (parts.Length != 2) throw new Exception($"Invalid cookie format: '{x}'.");
			return new Cookie(parts[0], parts[1]);
		});
	}

	public async Task<Cookie?> GetAsync(string key)
	{
		var cookies = await GetAllAsync();
		return cookies.FirstOrDefault(x => x.Key == key);
	}

	public async Task SetAsync(Cookie cookie, CancellationToken cancellationToken = default)
	{
		await SetAsync(cookie.Key, cookie.Value, cookie.Expiration, cancellationToken);
	}

	public async Task SetAsync(string key, string value, DateTimeOffset? expiration, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(key)) throw new Exception("Key is required when setting a cookie.");
		await JSRuntime.InvokeVoidAsync("eval", $"document.cookie = \"{key}={value}; expires={expiration:R}; path=/\"");
	}

	public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(key)) throw new Exception("Key is required when removing a cookie.");
		await JSRuntime.InvokeVoidAsync("eval", $"document.cookie = \"{key}=; expires=Thu, 01 Jan 1970 00:00:01 GMT; path=/\"");
	}
}