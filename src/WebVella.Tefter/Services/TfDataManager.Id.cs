using NpgsqlTypes;

namespace WebVella.Tefter;

public partial interface ITfDataManager
{
	/// <summary>
	/// Gets unique identifier for specified text
	/// </summary>
	/// <param name="textId"></param>
	/// <returns></returns>
	Result<Guid> GetId(
		params string[] textId);

	/// <summary>
	/// Gets unique identifier for provider argument. Argument is converted to text and result is for it.
	/// </summary>
	/// <param name="guidId"></param>
	/// <returns></returns>
	Result<Guid> GetId(
		Guid guidId);

	internal Result BulkFillIds(
		Dictionary<string, Guid> input);

	internal string CombineKey(params string[] keys);

	internal string CombineKey(List<string> keys);
}

public partial class TfDataManager
{
	private static AsyncLock _lock = new AsyncLock();
	private static Dictionary<string, Guid> idsDict = new Dictionary<string, Guid>();

	private const string SQL = "SELECT * FROM _tefter_id_dict_insert_select( @text_id, @id )";

	/// <summary>
	/// Gets unique identifier for specified text
	/// </summary>
	/// <param name="textId"></param>
	/// <returns></returns>
	public Result<Guid> GetId(
		params string[] textId)
	{
		try
		{
			using (_lock.Lock())
			{
				CheckInitIdDict();

				string combinedTextId = CombineKey(textId);

				if (idsDict.ContainsKey(combinedTextId))
					return idsDict[combinedTextId];


				var dt = _dbService.ExecuteSqlQueryCommand(SQL,
				new NpgsqlParameter("text_id", combinedTextId),
				new NpgsqlParameter("id", DBNull.Value));

				Guid id = (Guid)dt.Rows[0][0];

				idsDict[combinedTextId] = id;

				return Result.Ok(id);
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get GUID for specified text identifier.").CausedBy(ex));
		}
	}


	/// <summary>
	/// Gets unique identifier for provider argument. Argument is converted to text and result is for it.
	/// </summary>
	/// <param name="guidId"></param>
	/// <returns></returns>
	public Result<Guid> GetId(
		Guid guidId)
	{
		return GetId(guidId.ToString());
	}

	/// <summary>
	/// Fill dictionary with ids, used for synchronization
	/// </summary>
	/// <param name="input"></param>
	/// <returns></returns>
	public Result BulkFillIds(
		Dictionary<string, Guid> input)
	{
		try
		{
			using (_lock.Lock())
			{
				CheckInitIdDict();

				if (input is null)
					return Result.Ok();

				Dictionary<string, Guid> notFoundDict = new Dictionary<string, Guid>();

				foreach (var key in input.Keys)
				{
					if (idsDict.ContainsKey(key))
					{
						input[key] = idsDict[key];
						continue;
					}
					notFoundDict.Add(key, Guid.NewGuid());
				}

				var parameterIds = new NpgsqlParameter("@id", NpgsqlDbType.Array | NpgsqlDbType.Uuid);
				parameterIds.Value = new List<Guid>();

				var parameterTextIds = new NpgsqlParameter("@text_id", NpgsqlDbType.Array | NpgsqlDbType.Text);
				parameterTextIds.Value = new List<string>();

				foreach (var key in notFoundDict.Keys)
				{
					((List<Guid>)parameterIds.Value).Add(notFoundDict[key]);
					((List<string>)parameterTextIds.Value).Add(key);
				}

				var sql = $"INSERT INTO id_dict ( id, text_id ) SELECT * FROM UNNEST ( @id, @text_id ) ";

				_dbService.ExecuteSqlNonQueryCommand(sql, parameterIds, parameterTextIds);

				foreach (var key in notFoundDict.Keys)
				{
					input[key] = notFoundDict[key];
				}

				return Result.Ok();
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to generate and fill bulk shared key id value.").CausedBy(ex));
		}
	}


	private void CheckInitIdDict()
	{
		if (idsDict.Count > 0)
		{
			return;
		}

		var dt = _dbService.ExecuteSqlQueryCommand("SELECT * FROM id_dict");

		foreach (DataRow dr in dt.Rows)
		{
			Guid id = dr.Field<Guid>("id");
			string textId = dr.Field<string>("text_id");

			if (!idsDict.ContainsKey(textId))
			{
				idsDict[textId] = id;
			}
		}
	}


	public string CombineKey(params string[] keys)
	{
		if (keys == null || keys.Length == 0)
			return string.Empty;

		return string.Join(Constants.SHARED_KEY_SEPARATOR, keys) ?? string.Empty;
	}

	public string CombineKey(params List<string> keys)
	{
		return CombineKey(keys?.ToArray());
	}
}
