namespace WebVella.Tefter.Database.Dbo;
internal partial class DboManager
{
	public static Dictionary<Type, MemoryCacheOptions> cacheOptionsDict { get; internal set; } = new Dictionary<Type, MemoryCacheOptions>();
	public static Dictionary<Type, MemoryCache> cacheDict { get; internal set; } = new Dictionary<Type, MemoryCache>();

	private static void InitCache(Type type, MemoryCacheOptions options)
	{
		cacheOptionsDict.Add(type, options);
		cacheDict.Add(type, new MemoryCache(options));
	}

	private void AddToCache<T>(string key, DataTable obj) where T : class, new()
	{
		var memCache = cacheDict[typeof(T)];
		var options = new MemoryCacheEntryOptions();
		options.SetAbsoluteExpiration(TimeSpan.FromHours(1));
		//the error was registered in logs "Cannot access a disposed object. Object name: 'Microsoft.Extensions.Caching.Memory.MemoryCache'."
		//ignore error that occurs when memCache is disposed
		//in next attempt cache will not get this key and it will be set then
		try { memCache.Set(key, obj, options); } catch { };

	}

	private DataTable GetFromCache<T>(string key) where T : class, new()
	{
		var memCache = cacheDict[typeof(T)];
		DataTable result = null;
		bool found = memCache.TryGetValue(key, out result);
		return found ? result : null;
	}

	private void ClearCache<T>()
	{
		var meta = GetModelMeta<T>();
		var oldCache = cacheDict[typeof(T)];
		var memCache = new MemoryCache(meta.CacheOptions);
		cacheDict[typeof(T)] = memCache;
		oldCache.Dispose();
		//commented because fill logs
		//logger.LogDebug($"Clear-Cache:{typeof(T)}");
	}

	public static void ClearFullCache()
	{
		var oldCacheDict = cacheDict;

		var newCacheDict = new Dictionary<Type, MemoryCache>();
		foreach (var type in oldCacheDict.Keys)
			newCacheDict.Add(type, new MemoryCache(cacheOptionsDict[type]));

		cacheDict = newCacheDict;

	}

	private static string GetHashCode(string sql, params NpgsqlParameter[] parameters)
	{
		StringBuilder stringBuilder = new StringBuilder();

		stringBuilder.Append(GetObjectHashCode(sql ?? string.Empty));
		foreach (NpgsqlParameter param in parameters)
		{
			stringBuilder.Append(GetObjectHashCode(param.ParameterName));
			stringBuilder.Append(GetObjectHashCode(param.Value));
		}
		return stringBuilder.ToString();

	}

	private static string GetHashCode(string sql, List<NpgsqlParameter> parameters)
	{
		return GetHashCode(sql, parameters.ToArray());
	}

	private static string GetObjectHashCode(object obj)
	{
		if (obj is null)
			return String.Empty;
		if (obj is DBNull)
			return String.Empty;

		var type = obj.GetType();
		var stringValue = string.Empty;
		if (type.IsPrimitive || type == typeof(Decimal) || type == typeof(String) || type == typeof(Guid) || type == typeof(Decimal?) || type == typeof(Guid?) || type == typeof(DateOnly) || type == typeof(DateOnly?))
		{
			stringValue = obj.ToString();
		}
		else
		{
			stringValue = System.Text.Json.JsonSerializer.Serialize(obj);
		}

		return GetMd5Hash(stringValue);
	}

	static string GetMd5Hash(string input)
	{
		if (string.IsNullOrWhiteSpace(input))
			return string.Empty;

		byte[] data = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));

		StringBuilder sBuilder = new StringBuilder();
		for (int i = 0; i < data.Length; i++)
			sBuilder.Append(data[i].ToString("x2"));

		return sBuilder.ToString();
	}
}
