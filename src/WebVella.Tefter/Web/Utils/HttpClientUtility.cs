namespace WebVella.Tefter.Web.Utility;
public class HttpClientUtility
{
	private static string BASE_URL = "";
	public HttpClientUtility(ITfConfigurationService config)
	{
		BASE_URL = config.BaseUrl;
		if(BASE_URL.EndsWith("/")) BASE_URL = BASE_URL.Substring(0,BASE_URL.Length - 1);
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
		if(uri == null)  return null;

		using (HttpClient httpClient = new HttpClient())
		{
			httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Tefter App");
			httpClient.Timeout = TimeSpan.FromSeconds(20);

			using (HttpResponseMessage response = await httpClient.GetAsync(uri).ConfigureAwait(false))
			{
				HttpContent content = response.Content;
				var html = await content.ReadAsStringAsync();
				string title = Regex.Match(html, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
				return title;
			}
		}
	}

}
