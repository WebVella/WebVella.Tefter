using NpgsqlTypes;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	Guid GetId(
		params string[] textId);

	Guid GetId(
		Guid guidId);

	internal string CombineKey(
		params string[] keys);

	internal string CombineKey(
		List<string> keys);

	internal Task LoadIdsCacheAsync(
		CancellationToken stoppingToken);
}

public partial class TfService : ITfService
{
	public Guid GetId(
		params string[] textId)
	{
		try
		{
			string key = CombineKey(textId);

			if (_cache.TryGetValue(key, out Guid id))
				return id;

			var dt = _dbService.ExecuteSqlQueryCommand(
				"SELECT * FROM _tefter_id_dict_insert_select( @text_id, @id )",
				new NpgsqlParameter("text_id", key),
				new NpgsqlParameter("id", DBNull.Value));


			var cacheEntryOptions = new MemoryCacheEntryOptions()
				.SetSlidingExpiration(TimeSpan.FromDays(365));

			_cache.Set(key, (Guid)dt.Rows[0][0], cacheEntryOptions);

			return (Guid)dt.Rows[0][0];
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public Guid GetId(
		Guid guidId)
	{
		try
		{
			return GetId(guidId.ToString());
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	} 

	public string CombineKey(
		params string[] keys)
	{
		try
		{
			if (keys == null || keys.Length == 0)
				return string.Empty;

			return string.Join(TfConstants.SHARED_KEY_SEPARATOR, keys) ?? string.Empty;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public string CombineKey(
		params List<string> keys)
	{
		try
		{
			return CombineKey(keys?.ToArray());
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public Task LoadIdsCacheAsync(
		CancellationToken stoppingToken)
	{
		int pageSize = 10000;
		Guid? lastId = null;
		int rowsCount = 0;
		int loadedCount = 0;
		do
		{
			if (stoppingToken.IsCancellationRequested)
				return Task.CompletedTask;

			DataTable dt = GetKeysDataTable(lastId, pageSize);

			rowsCount = dt.Rows.Count;
			foreach (DataRow dr in dt.Rows)
			{
				var key = (string)dr["text_id"];
				var id = (Guid)dr["id"];
				_cache.Set(key, id);
				lastId = id;
			}
			
			loadedCount += rowsCount;
			//_logger.LogDebug("Ids total loaded: " + loadedCount);

		} while (rowsCount > 0);
		return Task.CompletedTask;
	}

	private DataTable GetKeysDataTable(Guid? lastId = null, int pageSize = 1000)
	{
		NpgsqlParameter idParam = new NpgsqlParameter("id", DbType.Guid);
		idParam.Value = lastId.HasValue? lastId.Value : DBNull.Value;
		return _dbService.ExecuteSqlQueryCommand(
			$"SELECT * FROM tf_id_dict WHERE ( id > @id OR @id IS NULL ) ORDER BY id LIMIT {pageSize}",idParam);
	}
}
