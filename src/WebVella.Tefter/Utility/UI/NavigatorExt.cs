namespace WebVella.Tefter.Utility;
public static partial class NavigatorExt
{
	public static bool UrlHasState(this NavigationManager navigator)
	{
		Uri uri = new Uri(navigator.Uri);
		var nodes = uri.LocalPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
		if (nodes.Length == 0) return TfConstants.SupportedUriFirstNodes.Contains(String.Empty);

		var firstNode = nodes[0].ToLowerInvariant();
		if (TfConstants.SupportedUriFirstNodes.Any(x => x.ToLowerInvariant() == firstNode)) return true;

		return false;
	}

	public static TfNavigationState GetRouteState(this NavigationManager navigator, string? url = null)
	{
		Uri? uri = null;
		if (String.IsNullOrWhiteSpace(url))
			uri = new Uri(navigator.Uri);
		else if (url.ToLowerInvariant().StartsWith("http"))
		{
			uri = new Uri(url);
		}
		else
		{
			if (url.StartsWith("/")) url = url.Substring(1, url.Length - 1);

			url = navigator.BaseUri + url;
			uri = new Uri(url);
		}

		return uri.GetNodeData();
	}

	public static async Task ApplyChangeToUrlQuery(this NavigationManager navigator, Dictionary<string, object?> replaceDict, bool forceLoad = false)
	{
		var currentUrl = navigator.Uri;
		var uri = new System.Uri(currentUrl);
		var queryDictionary = System.Web.HttpUtility.ParseQueryString(uri.Query);

		var newQueryDictionary = new Dictionary<string, string?>();
		foreach (string key in queryDictionary.Keys)
		{
			if (!replaceDict.Keys.Contains(key))
			{
				var queryValue = queryDictionary[key];
				if (!string.IsNullOrWhiteSpace(queryValue))
					newQueryDictionary[key] = queryValue;
			}
		}

		foreach (string key in replaceDict.Keys)
		{
			var queryValue = replaceDict[key];
			if (queryValue is null)
				continue;

			if (IsList(queryValue))
			{
				var asIList = (IList)queryValue;
				if (asIList.Count > 0)
				{
					var firstElement = asIList[0];
					if (firstElement is string)
					{
						var encodedList = new List<string>();
						foreach (var value in asIList)
						{
							encodedList.Add(ProcessQueryValueForUrl((string)value) ?? String.Empty);
						}
						if (encodedList.Count > 0)
						{
							newQueryDictionary[key] = String.Join(",", encodedList);
						}
					}
					else if (firstElement is int)
					{
						var encodedList = new List<string>();
						foreach (var value in asIList)
						{
							encodedList.Add(((int)value).ToString());
						}
						if (encodedList.Count > 0)
						{
							newQueryDictionary[key] = String.Join(",", encodedList);
						}
					}
					else if (firstElement is Enum)
					{
						var encodedList = new List<string>();
						foreach (var value in asIList)
						{
							encodedList.Add(((int)value).ToString());
						}
						if (encodedList.Count > 0)
						{
							newQueryDictionary[key] = String.Join(",", encodedList);
						}
					}
				}
			}
			else if (queryValue is int)
			{
				var value = (int)queryValue;
				newQueryDictionary[key] = ((int)value).ToString();
			}
			else if (queryValue is Enum)
			{
				var value = (int)queryValue;
				newQueryDictionary[key] = ((int)value).ToString();
			}
			else if (queryValue is string)
			{
				var value = (string)queryValue;
				if (!string.IsNullOrWhiteSpace(value))
					newQueryDictionary[key] = ProcessQueryValueForUrl(value);
			}
			else if (queryValue is Guid?)
			{
				var value = (Guid?)queryValue;
				if (value is not null)
					newQueryDictionary[key] = value.Value.ToString();
			}
			else if (queryValue is bool?)
			{
				var value = (bool?)queryValue;
				if (value is not null)
					newQueryDictionary[key] = value.Value.ToString();
			}
			else if (queryValue is List<TfFilterQuery>)
			{
				var value = (List<TfFilterQuery>)queryValue;
				if (value is not null)
					newQueryDictionary[key] = value.SerializeFiltersForUrl();
			}
			else if (queryValue is List<TfSortQuery>)
			{
				var value = (List<TfSortQuery>)queryValue;
				if (value is not null)
					newQueryDictionary[key] = value.SerializeSortsForUrl();
			}
			else
				throw new Exception("Query type not supported by utility method");
		}

		var queryStringList = new List<string>();
		foreach (var key in newQueryDictionary.Keys)
		{
			queryStringList.Add($"{key}={newQueryDictionary[key]}");
		}
		var urlQueryString = "";
		if (queryStringList.Count > 0)
			urlQueryString = "?" + string.Join("&", queryStringList);
		navigator.NavigateTo(uri.LocalPath + urlQueryString, forceLoad);
		if (!forceLoad)
			await Task.Delay(1);

	}

	public static Dictionary<string, string> GetQueryDictionaryFromUrl(string url)
	{
		var uri = new Uri(url);
		var queryDictionary = HttpUtility.ParseQueryString(uri.Query);

		var newQueryDictionary = new Dictionary<string, string>();
		foreach (string key in queryDictionary.Keys)
		{
			var queryValue = queryDictionary[key];
			if (!string.IsNullOrWhiteSpace(queryValue))
				newQueryDictionary[key] = queryValue;
		}

		return newQueryDictionary;
	}

	public static async Task ApplyChangeToUrlQuery(this NavigationManager navigator, string paramName, object? value, bool forceLoad = false)
	{
		var queryDict = new Dictionary<string, object?>();
		queryDict[paramName] = value;
		await navigator.ApplyChangeToUrlQuery(queryDict, forceLoad);
	}

	public static Dictionary<string, string?> ParseQueryString(string queryString)
	{
		var nvc = HttpUtility.ParseQueryString(queryString);
		var result = new Dictionary<string, string?>();
		if (nvc is null) return result;
		foreach (var key in nvc.AllKeys)
		{
			if (String.IsNullOrWhiteSpace(key)) continue;
			result[key] = nvc[key];
		}
		return result;
	}

	//string
	public static string? GetStringFromQuery(this NavigationManager navigator, string paramName, string? defaultValue = null)
	{
		return GetStringFromQuery(new Uri(navigator.Uri), paramName, defaultValue);
	}
	public static string? GetStringFromQuery(Uri uri, string paramName, string? defaultValue = null)
	{
		var queryDict = ParseQueryString(uri.Query);
		if (queryDict.ContainsKey(paramName))
			return ProcessQueryValueFromUrl(queryDict[paramName]) ?? defaultValue;
		return defaultValue;
	}
	//list<string>
	public static List<string>? GetStringListFromQuery(this NavigationManager navigator, string paramName, List<string>? defaultValue = null)
	{
		return GetStringListFromQuery(new Uri(navigator.Uri), paramName, defaultValue);
	}
	public static List<string>? GetStringListFromQuery(Uri uri, string paramName, List<string>? defaultValue = null)
	{
		//We use comma separated before encoding
		var paramValue = GetStringFromQuery(uri, paramName, null);
		if (String.IsNullOrWhiteSpace(paramValue))
			return defaultValue;

		return paramValue.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
	}
	//guid
	public static Guid? GetGuidFromQuery(this NavigationManager navigator, string paramName, Guid? defaultValue = null)
	{
		return GetGuidFromQuery(new Uri(navigator.Uri), paramName, defaultValue);
	}
	public static Guid? GetGuidFromQuery(Uri uri, string paramName, Guid? defaultValue = null)
	{
		if (Guid.TryParse(GetStringFromQuery(uri, paramName), out Guid outGuid))
		{
			return outGuid;
		}
		return defaultValue;
	}

	//boolean
	public static bool? GetBooleanFromQuery(this NavigationManager navigator, string paramName, bool? defaultValue = null)
	{
		return GetBooleanFromQuery(new Uri(navigator.Uri), paramName, defaultValue);
	}
	public static bool? GetBooleanFromQuery(Uri uri, string paramName, bool? defaultValue = null)
	{
		if (Boolean.TryParse(GetStringFromQuery(uri, paramName), out bool outBool))
		{
			return outBool;
		}
		return defaultValue;
	}

	//dateTime
	public static DateTime? GetDateTimeFromQuery(this NavigationManager navigator, string paramName, DateTime? defaultValue = null)
	{
		return GetDateTimeFromQuery(new Uri(navigator.Uri), paramName, defaultValue);
	}
	public static DateTime? GetDateTimeFromQuery(Uri uri, string paramName, DateTime? defaultValue = null)
	{
		var urlValue = GetStringFromQuery(uri, paramName, null);

		if (urlValue == "null")
		{
			return null;
		}
		else if (!String.IsNullOrWhiteSpace(urlValue))
		{
			return DateTimeUtils.FromUrlString(urlValue);
		}
		return defaultValue;
	}

	//dateOnly
	public static DateOnly? GetDateOnlyFromQuery(this NavigationManager navigator, string paramName, DateOnly? defaultValue = null)
	{
		return GetDateOnlyFromQuery(new Uri(navigator.Uri), paramName, defaultValue);
	}
	public static DateOnly? GetDateOnlyFromQuery(Uri uri, string paramName, DateOnly? defaultValue = null)
	{
		var urlValue = GetStringFromQuery(uri, paramName, null);

		if (urlValue == "null")
		{
			return null;
		}
		else if (!String.IsNullOrWhiteSpace(urlValue))
		{
			return DateOnlyUtils.FromUrlString(urlValue);
		}
		return defaultValue;
	}

	//int
	public static int? GetIntFromQuery(this NavigationManager navigator, string paramName, int? defaultValue = null)
	{
		return GetIntFromQuery(new Uri(navigator.Uri), paramName, defaultValue);
	}
	public static int? GetIntFromQuery(Uri uri, string paramName, int? defaultValue = null)
	{
		if (int.TryParse(GetStringFromQuery(uri, paramName), out int outInt))
		{
			return outInt;
		}
		return defaultValue;
	}

	//enum
	public static TEnum? GetEnumFromQuery<TEnum>(this NavigationManager navigator, string paramName, TEnum? defaultValue = null) where TEnum : struct
	{
		return GetEnumFromQuery<TEnum>(new Uri(navigator.Uri), paramName, defaultValue);
	}
	public static TEnum? GetEnumFromQuery<TEnum>(Uri uri, string paramName, TEnum? defaultValue = null) where TEnum : struct
	{
		var stringValue = GetStringFromQuery(uri, paramName);

		if (Enum.TryParse<TEnum>(stringValue, out TEnum outInt))
		{
			return outInt;
		}
		return defaultValue;
	}

	//List<int>
	public static List<int>? GetListIntFromQuery(this NavigationManager navigator, string paramName, List<int>? defaultValue = null)
	{
		return GetListIntFromQuery(new Uri(navigator.Uri), paramName, defaultValue);
	}
	public static List<int>? GetListIntFromQuery(Uri uri, string paramName, List<int>? defaultValue = null)
	{

		var stringValues = GetStringListFromQuery(uri, paramName, null);
		if (stringValues is null)
			return defaultValue;

		var result = new List<int>();
		foreach (var stringValue in stringValues)
		{
			if (int.TryParse(stringValue, out int outInt))
			{
				result.Add(outInt);
			}
		}
		if (result.Count == 0)
			return defaultValue;

		return result;
	}

	//List<Enum>
	public static List<TEnum>? GetListEnumFromQuery<TEnum>(this NavigationManager navigator, string paramName, List<TEnum>? defaultValue = null) where TEnum : struct
	{
		return GetListEnumFromQuery<TEnum>(new Uri(navigator.Uri), paramName, defaultValue);
	}
	public static List<TEnum>? GetListEnumFromQuery<TEnum>(Uri uri, string paramName, List<TEnum>? defaultValue = null) where TEnum : struct
	{

		var stringValues = GetStringListFromQuery(uri, paramName, null);
		if (stringValues is null)
			return defaultValue;

		var result = new List<TEnum>();
		foreach (var stringValue in stringValues)
		{
			if (Enum.TryParse<TEnum>(stringValue, out TEnum outInt))
			{
				result.Add(outInt);
			}
		}
		if (result.Count == 0)
			return defaultValue;

		return result;
	}

	public static string AddQueryValueToUri(string url, string paramName, string value)
	{
		var uri = new Uri($"http://localhost{url}");
		var queryDictionary = System.Web.HttpUtility.ParseQueryString(uri.Query);
		queryDictionary[paramName] = value;

		return uri.LocalPath + "?" + queryDictionary.ToString();
	}

	public static string? ProcessQueryValueFromUrl(string? queryValue)
	{
		if (String.IsNullOrWhiteSpace(queryValue))
			return null;

		return UrlUndoReplaceSpecialCharacters(HttpUtility.UrlDecode(queryValue));

	}
	public static string? ProcessQueryValueForUrl(string? queryValue)
	{
		var processedValue = queryValue?.Trim();
		if (String.IsNullOrWhiteSpace(processedValue))
			return null;

		return HttpUtility.UrlEncode(UrlReplaceSpecialCharacters(processedValue));

	}
	public static string UrlReplaceSpecialCharacters(string inputValue)
	{
		return inputValue.Replace("/", "§");
	}
	public static string UrlUndoReplaceSpecialCharacters(string inputValue)
	{
		return inputValue.Replace("§", "/");
	}

	public static string? SerializeFiltersForUrl(this List<TfFilterQuery>? filters, bool shouldProcess = true)
	{
		if (shouldProcess)
			return ProcessQueryValueForUrl(JsonSerializer.Serialize(filters));

		return JsonSerializer.Serialize(filters);
	}

	public static List<TfFilterQuery> DeserializeFiltersFromUrl(this string queryValue, bool isProcessed = false)
	{
		var items = JsonSerializer.Deserialize<List<TfFilterQuery>>(isProcessed ? queryValue : (ProcessQueryValueFromUrl(queryValue) ?? String.Empty));
		if (items == null) return new List<TfFilterQuery>();
		//Assign parents
		foreach (var item in items)
		{
			item.AssignParent(null);
		}
		return items;
	}

	public static void AssignParent(this TfFilterQuery node, TfFilterQuery? parent)
	{
		node.Parent = parent;
		foreach (var item in node.Items ?? new List<TfFilterQuery>())
		{
			item.AssignParent(node);
		}
	}

	public static TfFilterQuery? GetNodeByPath(this List<TfFilterQuery> items, List<string> path)
	{
		if (items == null || items.Count == 0 || path is null || path.Count == 0) return null;

		foreach (var item in items ?? new List<TfFilterQuery>())
		{
			var match = CheckNodeForPathMatch(item, path);
			if (match is not null) return match;
		}
		return null;
	}

	public static TfFilterQuery? CheckNodeForPathMatch(TfFilterQuery node, List<string> path)
	{
		if (node.Path.MatchesList(path)) return node;
		foreach (var item in node.Items ?? new List<TfFilterQuery>())
		{
			var match = CheckNodeForPathMatch(item, path);
			if (match is not null) return match;
		}

		return null;
	}

	public static string? SerializeSortsForUrl(this List<TfSortQuery>? sorts, bool shouldProcess = true)
	{
		if (shouldProcess)
			return ProcessQueryValueForUrl(JsonSerializer.Serialize(sorts));

		return JsonSerializer.Serialize(sorts);
	}

	public static List<TfSortQuery> DeserializeSortsFromUrl(this string queryValue, bool isProcessed = false)
	{
		var items = JsonSerializer.Deserialize<List<TfSortQuery>>(isProcessed ? queryValue : (ProcessQueryValueFromUrl(queryValue) ?? String.Empty));
		if (items == null) return new List<TfSortQuery>();
		return items;
	}

	public static string GetLocalUrl(this NavigationManager navigator)
	{
		var uri = new Uri(navigator.Uri);
		var localUrl = uri.LocalPath;
		if (!String.IsNullOrWhiteSpace(uri.Query))
			localUrl = localUrl + uri.Query;

		return localUrl;
	}

	public static string? GenerateWithLocalAsReturnUrl(this string mainUrl, string? returnUrl)
	{
		if (String.IsNullOrWhiteSpace(mainUrl)) return null;
		if (String.IsNullOrWhiteSpace(returnUrl)) return mainUrl;
		mainUrl = !mainUrl.StartsWith("http") ? $"http://localhost{mainUrl}" : mainUrl;
		returnUrl = !returnUrl.StartsWith("http") ? $"http://localhost{returnUrl}" : returnUrl;
		
		var returnUri = new Uri(returnUrl);
		var mainUri = new Uri(mainUrl);
		var queryDictionary = System.Web.HttpUtility.ParseQueryString(mainUri.Query);
		queryDictionary[TfConstants.ReturnUrlQueryName] = ProcessQueryValueForUrl(returnUri.PathAndQuery);

		return mainUri.LocalPath + "?" + queryDictionary.ToString();		
	}

	public static void ReloadCurrentUrl(this NavigationManager navigator)
	{
		navigator.NavigateTo(navigator.Uri, true);
	}
	public static bool IsList(object o)
	{
		if (o == null) return false;
		return o is IList &&
			   o.GetType().IsGenericType &&
			   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
	}

	public static bool IsDictionary(object o)
	{
		if (o == null) return false;
		return o is IDictionary &&
			   o.GetType().IsGenericType &&
			   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>));
	}

	public static void NotFound(this NavigationManager navigator)
	{
		navigator.NavigateTo(TfConstants.NotFoundPageUrl, true);
	}

	public static bool IsSpaceViewSavedUrlChanged(this NavigationManager navigator, string url)
	{
		var savedUri = new Uri($"http://localhost{url}");
		var currentUri = new Uri(navigator.Uri);

		if (savedUri.LocalPath != currentUri.LocalPath) return true;

		var savedQueryDict = HttpUtility.ParseQueryString(savedUri.Query);
		var currentQueryDict = HttpUtility.ParseQueryString(currentUri.Query);

		var savedSortedDict = new SortedDictionary<string, string?>();
		var currentSortedDict = new SortedDictionary<string, string?>();

		foreach (string? key in savedQueryDict.AllKeys)
		{
			if (String.IsNullOrWhiteSpace(key)) continue;
			if (key == TfConstants.TabQueryName) continue;
			if (key == TfConstants.ActiveSaveQueryName) continue;
			savedSortedDict[key] = savedQueryDict[key];
		}
		foreach (string? key in currentQueryDict.AllKeys)
		{
			if (String.IsNullOrWhiteSpace(key)) continue;
			if (key == TfConstants.TabQueryName) continue;
			if (key == TfConstants.ActiveSaveQueryName) continue;
			currentSortedDict[key] = currentQueryDict[key];
		}
		if (savedSortedDict.Keys.Count != currentSortedDict.Keys.Count) return true;

		foreach (var key in savedSortedDict.Keys)
		{
			if (!currentSortedDict.ContainsKey(key) || savedSortedDict[key] != currentSortedDict[key])
				return true;
		}
		return false;
	}
	public static string GenerateQueryName()
	{
		return "q" + (Guid.NewGuid()).ToString().Split("-")[0];
	}
}
