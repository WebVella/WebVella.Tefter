namespace WebVella.Tefter.Web.Utils;

internal static class NavigatorExt
{
	internal static Models.RouteData GetUrlData(this NavigationManager navigator, string url = null)
	{
		Guid? spaceId = null;
		Guid? spaceDataId = null;
		Guid? spaceViewId = null;
		Guid? userId = null;
		Guid? dataProviderId = null;
		Dictionary<int, string> pathDict = new();
		Uri uri = null;
		if (String.IsNullOrWhiteSpace(url))
			uri = new Uri(navigator.Uri);
		else
			uri = new Uri(url);

		var nodes = uri.LocalPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
		var dictIndex = 0;
		foreach (var item in nodes)
		{
			pathDict[dictIndex] = item;
			dictIndex++;
		}

		if (uri.LocalPath.StartsWith("/space/"))
		{
			if (pathDict.ContainsKey(1))
			{
				if (Guid.TryParse(pathDict[1], out Guid outGuid)) spaceId = outGuid;
			}

			if (pathDict.ContainsKey(3))
			{
				if (Guid.TryParse(pathDict[3], out Guid outGuid)) spaceDataId = outGuid;
			}

			if (pathDict.ContainsKey(5))
			{
				if (Guid.TryParse(pathDict[5], out Guid outGuid)) spaceViewId = outGuid;
			}

		}

		else if (uri.LocalPath.StartsWith("/admin/users/"))
		{
			if (pathDict.ContainsKey(2)
				&& Guid.TryParse(pathDict[2], out Guid outGuid))
				userId = outGuid;
		}

		if (uri.LocalPath.StartsWith("/admin/data-providers/"))
		{
			if (pathDict.ContainsKey(2)
				&& Guid.TryParse(pathDict[2], out Guid outGuid))
				dataProviderId = outGuid;
		}

		return new Models.RouteData
		{
			SegmentsByIndexDict = pathDict,
			SpaceId = spaceId,
			SpaceDataId = spaceDataId,
			SpaceViewId = spaceViewId,
			UserId = userId,
			DataProviderId = dataProviderId,
		};
	}


	internal static async Task ApplyChangeToUrlQuery(this NavigationManager navigator, Dictionary<string, object> replaceDict, bool forceLoad = false)
	{
		var currentUrl = navigator.Uri;
		var uri = new System.Uri(currentUrl);
		var queryDictionary = System.Web.HttpUtility.ParseQueryString(uri.Query);

		var newQueryDictionary = new Dictionary<string, string>();
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
							encodedList.Add(ProcessQueryValueForUrl((string)value));
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
					newQueryDictionary[key] = value.ToString();
			}
			else if (queryValue is bool?)
			{
				var value = (bool?)queryValue;
				if (value is not null)
					newQueryDictionary[key] = value.ToString();
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

	internal static Dictionary<string, string> GetQueryDictionaryFromUrl(string url)
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

	internal static async Task SetParamToUrlQuery(this NavigationManager navigator, string paramName, object value, bool forceLoad = false)
	{
		var queryDict = new Dictionary<string, object>();
		queryDict[paramName] = value;
		await navigator.ApplyChangeToUrlQuery(queryDict, forceLoad);
	}

	internal static Dictionary<string, string> ParseQueryString(string queryString)
	{
		var nvc = HttpUtility.ParseQueryString(queryString);
		return nvc.AllKeys.ToDictionary(k => k, k => nvc[k]);
	}

	internal static string GetStringFromQuery(this NavigationManager navigator, string paramName, string defaultValue = null)
	{
		var urlAbsolute = navigator.ToAbsoluteUri(navigator.Uri);
		var queryDict = ParseQueryString(urlAbsolute.Query);
		if (queryDict.ContainsKey(paramName))
			return ProcessQueryValueFromUrl(queryDict[paramName]) ?? defaultValue;
		return defaultValue;
	}

	internal static List<string> GetStringListFromQuery(this NavigationManager navigator, string paramName, List<string> defaultValue = null)
	{
		//We use comma separated before encoding
		var paramValue = navigator.GetStringFromQuery(paramName, null);
		if (String.IsNullOrWhiteSpace(paramValue))
			return defaultValue;

		return paramValue.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
	}

	internal static Guid? GetGuidFromQuery(this NavigationManager navigator, string paramName, Guid? defaultValue = null)
	{
		if (Guid.TryParse(navigator.GetStringFromQuery(paramName), out Guid outGuid))
		{
			return outGuid;
		}
		return defaultValue;
	}

	internal static bool? GetBooleanFromQuery(this NavigationManager navigator, string paramName, bool? defaultValue = null)
	{
		if (Boolean.TryParse(navigator.GetStringFromQuery(paramName), out bool outBool))
		{
			return outBool;
		}
		return defaultValue;
	}
	internal static DateTime? GetDateFromQuery(this NavigationManager navigator, string paramName, DateTime? defaultValue = null)
	{
		var urlValue = navigator.GetStringFromQuery(paramName, null);

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

	internal static DateOnly? GetDateOnlyFromQuery(this NavigationManager navigator, string paramName, DateOnly? defaultValue = null)
	{
		var urlValue = navigator.GetStringFromQuery(paramName, null);

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

	internal static int? GetIntFromQuery(this NavigationManager navigator, string paramName, int? defaultValue = null)
	{
		if (int.TryParse(navigator.GetStringFromQuery(paramName), out int outInt))
		{
			return outInt;
		}
		return defaultValue;
	}

	internal static TEnum? GetEnumFromQuery<TEnum>(this NavigationManager navigator, string paramName, TEnum? defaultValue = null) where TEnum : struct
	{
		var stringValue = navigator.GetStringFromQuery(paramName);

		if (Enum.TryParse<TEnum>(stringValue, out TEnum outInt))
		{
			return outInt;
		}
		return defaultValue;
	}

	internal static List<int> GetListIntFromQuery(this NavigationManager navigator, string paramName, List<int> defaultValue = null)
	{

		var stringValues = navigator.GetStringListFromQuery(paramName, null);
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

	internal static List<TEnum> GetListEnumFromQuery<TEnum>(this NavigationManager navigator, string paramName, List<TEnum> defaultValue = null) where TEnum : struct
	{

		var stringValues = navigator.GetStringListFromQuery(paramName, null);
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

	internal static string ProcessQueryValueFromUrl(string queryValue)
	{
		if (String.IsNullOrWhiteSpace(queryValue))
			return null;

		return UrlUndoReplaceSpecialCharacters(HttpUtility.UrlDecode(queryValue));

	}
	internal static string ProcessQueryValueForUrl(string queryValue)
	{
		var processedValue = queryValue?.Trim();
		if (String.IsNullOrWhiteSpace(processedValue))
			return null;

		return HttpUtility.UrlEncode(UrlReplaceSpecialCharacters(processedValue));

	}
	internal static string UrlReplaceSpecialCharacters(string inputValue)
	{
		return inputValue.Replace("/", "~");
	}
	internal static string UrlUndoReplaceSpecialCharacters(string inputValue)
	{
		return inputValue.Replace("~", "/");
	}

	internal static string GetLocalUrl(this NavigationManager navigator)
	{
		var uri = new Uri(navigator.Uri);
		var localUrl = uri.LocalPath;
		if (!String.IsNullOrWhiteSpace(uri.Query))
			localUrl = localUrl + uri.Query;

		return localUrl;
	}

	internal static void ReloadCurrentUrl(this NavigationManager navigator)
	{
		navigator.NavigateTo(navigator.Uri, true);
	}
	private static bool IsList(object o)
	{
		if (o == null) return false;
		return o is IList &&
			   o.GetType().IsGenericType &&
			   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
	}

	private static bool IsDictionary(object o)
	{
		if (o == null) return false;
		return o is IDictionary &&
			   o.GetType().IsGenericType &&
			   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>));
	}

	internal static void NotFound(this NavigationManager navigator)
	{
		navigator.NavigateTo(TfConstants.NotFoundPageUrl, true);
	}


}
