﻿namespace WebVella.Tefter.Web.Utils;
using WebVella.Tefter.Web.Models;
internal static class NavigatorExt
{
	internal static TfRouteState GetRouteState(this NavigationManager navigator, string url = null)
	{
		Uri uri = null;
		if (String.IsNullOrWhiteSpace(url))
			uri = new Uri(navigator.Uri);
		else
			uri = new Uri(url);
		return GetNodeData(uri);
	}

	internal static TfRouteState GetNodeData(Uri uri)
	{
		var result = new TfRouteState();

		var nodes = uri.LocalPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
		var dictIndex = 0;
		foreach (var item in nodes)
		{
			result.NodesDict[dictIndex] = item;
			dictIndex++;
		}
		if (result.NodesDict.Count == 0)
		{
			result = result with { FirstNode = RouteDataFirstNode.Home };
			result = result with { SecondNode = RouteDataSecondNode.Dashboard };
		}
		else if (result.NodesDict[0] == TfConstants.RouteNameAdmin)
		{
			result = result with { FirstNode = RouteDataFirstNode.Admin };
			result = result with { SecondNode = RouteDataSecondNode.Dashboard };
			//Process 2
			if (result.NodesDict.Count >= 2)
			{
				if (result.NodesDict[1] == TfConstants.RouteNameUsers)
				{
					result = result with { SecondNode = RouteDataSecondNode.Users };
				}
				else if (result.NodesDict[1] == TfConstants.RouteNameDataProviders)
				{
					result = result with { SecondNode = RouteDataSecondNode.DataProviders };
				}
				else if (result.NodesDict[1] == TfConstants.RouteNameSharedColumns)
				{
					result = result with { SecondNode = RouteDataSecondNode.SharedColumns };
				}
				//Process 3
				if (result.NodesDict.Count >= 3)
				{
					if (result.SecondNode == RouteDataSecondNode.Users)
					{
						if (Guid.TryParse(result.NodesDict[2], out Guid outGuid)) result = result with { UserId = outGuid };
					}
					else if (result.SecondNode == RouteDataSecondNode.DataProviders)
					{
						if (Guid.TryParse(result.NodesDict[2], out Guid outGuid)) result = result with { DataProviderId = outGuid };
					}
				}

				//Process 4
				if (result.NodesDict.Count >= 4)
				{
					if (result.NodesDict[3] == TfConstants.RouteNameAccess)
					{
						result = result with { ThirdNode = RouteDataThirdNode.Access };
					}
					else if (result.NodesDict[3] == TfConstants.RouteNameSaves)
					{
						result = result with { ThirdNode = RouteDataThirdNode.Saves };
					}
					else if (result.NodesDict[3] == TfConstants.RouteNameSchema)
					{
						result = result with { ThirdNode = RouteDataThirdNode.Schema };
					}
					else if (result.NodesDict[3] == TfConstants.RouteNameKeys)
					{
						result = result with { ThirdNode = RouteDataThirdNode.SharedKeys };
					}
					else if (result.NodesDict[3] == TfConstants.RouteNameAux)
					{
						result = result with { ThirdNode = RouteDataThirdNode.AuxColumns };
					}
					else if (result.NodesDict[3] == TfConstants.RouteNameSynchronization)
					{
						result = result with { ThirdNode = RouteDataThirdNode.Synchronization };
					}
					else if (result.NodesDict[3] == TfConstants.RouteNameData)
					{
						result = result with { ThirdNode = RouteDataThirdNode.Data };
					}
				}
			}

		}
		else if (result.NodesDict[0] == TfConstants.RouteNameSpace)
		{
			result = result with { FirstNode = RouteDataFirstNode.Space };
			//Process 2
			if (result.NodesDict.Count >= 2)
			{
				result = result with { SecondNode = RouteDataSecondNode.Dashboard };
				if (Guid.TryParse(result.NodesDict[1], out Guid outGuid)) result = result with { SpaceId = outGuid };
			}
			//Process 3
			if (result.NodesDict.Count >= 3)
			{
				if (result.NodesDict[2] == TfConstants.RouteNameSpacePage)
				{
					result = result with
					{
						SecondNode = RouteDataSecondNode.SpacePage,
						ThirdNode = RouteDataThirdNode.Details
					};
				}
				else if (result.NodesDict[2] == TfConstants.RouteNameSpaceView)
				{
					result = result with
					{
						SecondNode = RouteDataSecondNode.SpaceView,
						ThirdNode = RouteDataThirdNode.Details
					};
				}
				else if (result.NodesDict[2] == TfConstants.RouteNameSpaceData)
				{
					result = result with
					{
						SecondNode = RouteDataSecondNode.SpaceData,
						ThirdNode = RouteDataThirdNode.Details
					};
					if (result.NodesDict.Count >= 5)
					{
						if (result.NodesDict[4] == TfConstants.RouteNameViews)
						{
							result = result with
							{
								ThirdNode = RouteDataThirdNode.Views
							};
						}
						else if (result.NodesDict[4] == TfConstants.RouteNameData)
						{
							result = result with
							{
								ThirdNode = RouteDataThirdNode.Data
							};
						}
					}
				}
			}
			//Process 4
			if (result.NodesDict.Count >= 4)
			{
				if (result.SecondNode == RouteDataSecondNode.SpacePage)
				{
					if (Guid.TryParse(result.NodesDict[3], out Guid outGuid)) result = result with { SpacePageId = outGuid };
				}
				else if (result.SecondNode == RouteDataSecondNode.SpaceView)
				{
					if (Guid.TryParse(result.NodesDict[3], out Guid outGuid)) result = result with { SpaceViewId = outGuid };
				}
				else if (result.SecondNode == RouteDataSecondNode.SpaceData)
				{
					if (Guid.TryParse(result.NodesDict[3], out Guid outGuid)) result = result with { SpaceDataId = outGuid };
				}
			}
			//process 5
			if (result.NodesDict.Count >= 5)
			{
				if (result.NodesDict[4] == TfConstants.RouteNameManage)
				{
					result = result with { ThirdNode = RouteDataThirdNode.Manage };
				}
				else if (result.NodesDict[4] == TfConstants.RouteNameViews)
				{
					result = result with { ThirdNode = RouteDataThirdNode.Views };
				}
			}
		}
		else if (result.NodesDict[0] == TfConstants.RouteNamePages)
		{
			result = result with { FirstNode = RouteDataFirstNode.Pages };
		}
		//Query
		var page = GetIntFromQuery(uri, TfConstants.PageQueryName, 1);
		var pageSize = GetIntFromQuery(uri, TfConstants.PageSizeQueryName, null);
		var search = GetStringFromQuery(uri, TfConstants.SearchQueryName, null);
		List<TucFilterBase> filters = null;
		var filtersString = GetStringFromQuery(uri, TfConstants.FiltersQueryName, null);
		if (!String.IsNullOrWhiteSpace(filtersString)) filters = DeserializeFiltersFromUrl(filtersString, true);
		List<TucSort> sorts = null;
		var sortString = GetStringFromQuery(uri, TfConstants.SortsQueryName, null);
		if (!String.IsNullOrWhiteSpace(sortString)) sorts = DeserializeSortsFromUrl(sortString, true);

		Guid? activeSaveId = GetGuidFromQuery(uri, TfConstants.ActiveSaveQueryName, null);
		bool searchInBookmarks = GetBooleanFromQuery(uri, TfConstants.SearchInBookmarksQueryName, true).Value;
		bool searchInSaves = GetBooleanFromQuery(uri, TfConstants.SearchInSavesQueryName, true).Value;
		bool searchInViews = GetBooleanFromQuery(uri, TfConstants.SearchInViewsQueryName, true).Value;

		Guid? spaceViewPresetId = GetGuidFromQuery(uri,TfConstants.PresetIdQueryName, null);

		result = result with
		{
			Page = page,
			PageSize = pageSize,
			Search = search,
			Filters = filters,
			Sorts = sorts,
			ActiveSaveId = activeSaveId,
			SearchInBookmarks = searchInBookmarks,
			SearchInSaves = searchInSaves,
			SearchInViews = searchInViews,
			SpaceViewPresetId = spaceViewPresetId,
		};

		return result;
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
			else if (queryValue is List<TucFilterBase>)
			{
				var value = (List<TucFilterBase>)queryValue;
				if (value is not null)
					newQueryDictionary[key] = SerializeFiltersForUrl(value);
			}
			else if (queryValue is List<TucSort>)
			{
				var value = (List<TucSort>)queryValue;
				if (value is not null)
					newQueryDictionary[key] = SerializeSortsForUrl(value);
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

	//string
	internal static string GetStringFromQuery(this NavigationManager navigator, string paramName, string defaultValue = null)
	{
		return GetStringFromQuery(new Uri(navigator.Uri), paramName, defaultValue);
	}
	internal static string GetStringFromQuery(Uri uri, string paramName, string defaultValue = null)
	{
		var queryDict = ParseQueryString(uri.Query);
		if (queryDict.ContainsKey(paramName))
			return ProcessQueryValueFromUrl(queryDict[paramName]) ?? defaultValue;
		return defaultValue;
	}
	//list<string>
	internal static List<string> GetStringListFromQuery(this NavigationManager navigator, string paramName, List<string> defaultValue = null)
	{
		return GetStringListFromQuery(new Uri(navigator.Uri), paramName, defaultValue);
	}
	internal static List<string> GetStringListFromQuery(Uri uri, string paramName, List<string> defaultValue = null)
	{
		//We use comma separated before encoding
		var paramValue = GetStringFromQuery(uri, paramName, null);
		if (String.IsNullOrWhiteSpace(paramValue))
			return defaultValue;

		return paramValue.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
	}
	//guid
	internal static Guid? GetGuidFromQuery(this NavigationManager navigator, string paramName, Guid? defaultValue = null)
	{
		return GetGuidFromQuery(new Uri(navigator.Uri), paramName, defaultValue);
	}
	internal static Guid? GetGuidFromQuery(Uri uri, string paramName, Guid? defaultValue = null)
	{
		if (Guid.TryParse(GetStringFromQuery(uri, paramName), out Guid outGuid))
		{
			return outGuid;
		}
		return defaultValue;
	}

	//boolean
	internal static bool? GetBooleanFromQuery(this NavigationManager navigator, string paramName, bool? defaultValue = null)
	{
		return GetBooleanFromQuery(new Uri(navigator.Uri), paramName, defaultValue);
	}
	internal static bool? GetBooleanFromQuery(Uri uri, string paramName, bool? defaultValue = null)
	{
		if (Boolean.TryParse(GetStringFromQuery(uri, paramName), out bool outBool))
		{
			return outBool;
		}
		return defaultValue;
	}

	//dateTime
	internal static DateTime? GetDateTimeFromQuery(this NavigationManager navigator, string paramName, DateTime? defaultValue = null)
	{
		return GetDateTimeFromQuery(new Uri(navigator.Uri), paramName, defaultValue);
	}
	internal static DateTime? GetDateTimeFromQuery(Uri uri, string paramName, DateTime? defaultValue = null)
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
	internal static DateOnly? GetDateOnlyFromQuery(this NavigationManager navigator, string paramName, DateOnly? defaultValue = null)
	{
		return GetDateOnlyFromQuery(new Uri(navigator.Uri), paramName, defaultValue);
	}
	internal static DateOnly? GetDateOnlyFromQuery(Uri uri, string paramName, DateOnly? defaultValue = null)
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
	internal static int? GetIntFromQuery(this NavigationManager navigator, string paramName, int? defaultValue = null)
	{
		return GetIntFromQuery(new Uri(navigator.Uri), paramName, defaultValue);
	}
	internal static int? GetIntFromQuery(Uri uri, string paramName, int? defaultValue = null)
	{
		if (int.TryParse(GetStringFromQuery(uri, paramName), out int outInt))
		{
			return outInt;
		}
		return defaultValue;
	}

	//enum
	internal static TEnum? GetEnumFromQuery<TEnum>(this NavigationManager navigator, string paramName, TEnum? defaultValue = null) where TEnum : struct
	{
		return GetEnumFromQuery<TEnum>(new Uri(navigator.Uri), paramName, defaultValue);
	}
	internal static TEnum? GetEnumFromQuery<TEnum>(Uri uri, string paramName, TEnum? defaultValue = null) where TEnum : struct
	{
		var stringValue = GetStringFromQuery(uri, paramName);

		if (Enum.TryParse<TEnum>(stringValue, out TEnum outInt))
		{
			return outInt;
		}
		return defaultValue;
	}

	//List<int>
	internal static List<int> GetListIntFromQuery(this NavigationManager navigator, string paramName, List<int> defaultValue = null)
	{
		return GetListIntFromQuery(new Uri(navigator.Uri), paramName, defaultValue);
	}
	internal static List<int> GetListIntFromQuery(Uri uri, string paramName, List<int> defaultValue = null)
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
	internal static List<TEnum> GetListEnumFromQuery<TEnum>(this NavigationManager navigator, string paramName, List<TEnum> defaultValue = null) where TEnum : struct
	{
		return GetListEnumFromQuery<TEnum>(new Uri(navigator.Uri), paramName, defaultValue);
	}
	internal static List<TEnum> GetListEnumFromQuery<TEnum>(Uri uri, string paramName, List<TEnum> defaultValue = null) where TEnum : struct
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

	internal static string AddQueryValueToUri(string url, string paramName, string value)
	{
		var uri = new Uri($"http://localhost{url}");
		var queryDictionary = System.Web.HttpUtility.ParseQueryString(uri.Query);
		queryDictionary[paramName] = value;

		return uri.LocalPath + "?" + queryDictionary.ToString();
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
		return inputValue.Replace("/", "§");
	}
	internal static string UrlUndoReplaceSpecialCharacters(string inputValue)
	{
		return inputValue.Replace("§", "/");
	}

	internal static string SerializeFiltersForUrl(List<TucFilterBase> filters, bool shouldProcess = true)
	{
		var queryObject = new List<TucFilterQuery>();
		foreach (var item in filters)
		{
			queryObject.Add(TucFilterBase.ToQuery(item));
		}
		if (shouldProcess)
			return ProcessQueryValueForUrl(JsonSerializer.Serialize(queryObject));

		return JsonSerializer.Serialize(queryObject);
	}

	internal static List<TucFilterBase> DeserializeFiltersFromUrl(string queryValue, bool isProcessed = false)
	{
		var items = JsonSerializer.Deserialize<List<TucFilterQuery>>(isProcessed ? queryValue : ProcessQueryValueFromUrl(queryValue));
		if (items == null) return null;
		var result = new List<TucFilterBase>();
		foreach (var item in items)
		{
			result.Add(TucFilterBase.FromQuery(item));
		}
		return result;
	}

	internal static string SerializeSortsForUrl(List<TucSort> sorts, bool shouldProcess = true)
	{
		var queryObject = new List<TucSortQuery>();
		foreach (var item in sorts)
		{
			queryObject.Add(item.ToQuery());
		}
		if (shouldProcess)
			return ProcessQueryValueForUrl(JsonSerializer.Serialize(queryObject));

		return JsonSerializer.Serialize(queryObject);
	}

	internal static List<TucSort> DeserializeSortsFromUrl(string queryValue, bool isProcessed = false)
	{
		var items = JsonSerializer.Deserialize<List<TucSortQuery>>(isProcessed ? queryValue : ProcessQueryValueFromUrl(queryValue));
		if (items == null) return null;
		var result = new List<TucSort>();
		foreach (var item in items)
		{
			result.Add(new TucSort(item));
		}
		return result;
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

	internal static bool IsSpaceViewSavedUrlChanged(this NavigationManager navigator, string url)
	{
		var savedUri = new Uri($"http://localhost{url}");
		var currentUri = new Uri(navigator.Uri);

		if (savedUri.LocalPath != currentUri.LocalPath) return true;

		var savedQueryDict = HttpUtility.ParseQueryString(savedUri.Query);
		var currentQueryDict = HttpUtility.ParseQueryString(currentUri.Query);

		var savedSortedDict = new SortedDictionary<string, string>();
		var currentSortedDict = new SortedDictionary<string, string>();

		foreach (string key in savedQueryDict.AllKeys)
		{
			if (key == TfConstants.ActiveSaveQueryName) continue;
			savedSortedDict[key] = savedQueryDict[key];
		}
		foreach (string key in currentQueryDict.AllKeys)
		{
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

}
