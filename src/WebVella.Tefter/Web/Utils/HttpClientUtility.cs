namespace WebVella.Tefter.Web.Utility;
public class HttpClientUtility
{
	private static string BASE_URL = "";
	public HttpClientUtility(ITfConfigurationService config)
	{
		BASE_URL = config.BaseUrl;
		if (BASE_URL.EndsWith("/")) BASE_URL = BASE_URL.Substring(0, BASE_URL.Length - 1);
	}
	public async Task<string> GetMetaTitleFromUri(string url)
	{
		if (!url.StartsWith("http:") && String.IsNullOrWhiteSpace(BASE_URL))
			return null;

		if (url.StartsWith("/"))
		{
			url = BASE_URL + url;
		}
		Uri uri = null;
		if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri)) { }
		if (uri == null) return null;

		if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps) return null;

		using (HttpClient httpClient = new HttpClient())
		{
			httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Tefter App");
			httpClient.Timeout = TimeSpan.FromSeconds(20);

			using (HttpResponseMessage response = await httpClient.GetAsync(uri).ConfigureAwait(false))
			{
				if (!response.IsSuccessStatusCode) return null;
				HttpContent content = response.Content;
				var html = await content.ReadAsStringAsync();
				string title = Regex.Match(html, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
				return title;
			}
		}
	}

	public async Task<string> GetFavIconForUri(string url)
	{
		if (!url.StartsWith("http:") && String.IsNullOrWhiteSpace(BASE_URL))
			return null;

		if (url.StartsWith("/"))
		{
			url = BASE_URL + url;
		}
		Uri uri = null;
		if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri)) { }
		if (uri == null) return null;
		if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps) return null;

		using (HttpClient httpClient = new HttpClient())
		{
			httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Tefter App");
			httpClient.Timeout = TimeSpan.FromSeconds(2);

			//First try default favicon.ico
			var favIconPath = uri.GetLeftPart(System.UriPartial.Authority);
			favIconPath += "/favicon.ico";
			using (HttpResponseMessage response = await httpClient.GetAsync(uri).ConfigureAwait(false))
			{
				if (response.IsSuccessStatusCode)
				{
					return favIconPath;
				}
			}
			httpClient.Timeout = TimeSpan.FromSeconds(5);
			using (HttpResponseMessage response = await httpClient.GetAsync(uri).ConfigureAwait(false))
			{
				if (!response.IsSuccessStatusCode) return null;

				HttpContent content = response.Content;
				var html = await content.ReadAsStringAsync();
				var capture = Regex.Match(html, @"rel=['""](?:shortcut )?icon['""] href=['""]([^?'""]+)[?'""]", RegexOptions.IgnoreCase);
				
				if(capture.Groups is not null && capture.Groups.Count> 1) return capture.Groups[1].Value;
			}
		}

		return null;
	}

}
