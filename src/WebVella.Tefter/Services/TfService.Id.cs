using NpgsqlTypes;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	Guid GetId(
		params string[] textId);

	Guid GetId(
		Guid guidId);

	internal void BulkFillIds(
		Dictionary<string, Guid> input);

	internal string CombineKey(
		params string[] keys);

	internal string CombineKey(
		List<string> keys);
}

public partial class TfService : ITfService
{
	public Guid GetId(
		params string[] textId)
	{
		try
		{
			var dt = _dbService.ExecuteSqlQueryCommand(
				"SELECT * FROM _tefter_id_dict_insert_select( @text_id, @id )",
				new NpgsqlParameter("text_id", CombineKey(textId)),
				new NpgsqlParameter("id", DBNull.Value));

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

	public void BulkFillIds(
		Dictionary<string, Guid> input)
	{
		try
		{
			if (input is null || input.Keys.Count == 0)
				return;

			var parameterIds = new NpgsqlParameter("@id", NpgsqlDbType.Array | NpgsqlDbType.Uuid);
			parameterIds.Value = new List<Guid>();

			var parameterTextIds = new NpgsqlParameter("@text_id", NpgsqlDbType.Array | NpgsqlDbType.Text);
			parameterTextIds.Value = new List<string>();

			foreach (var key in input.Keys)
			{
				((List<Guid>)parameterIds.Value).Add(input[key]);
				((List<string>)parameterTextIds.Value).Add(key);
			}

			//select all by existing text_id
			var sql = "SELECT * FROM id_dict WHERE text_id = ANY(@text_id)";
			var dt = _dbService.ExecuteSqlQueryCommand(sql, parameterTextIds);

			//create dictionary with existing data
			Dictionary<string, Guid> foundIds = new Dictionary<string, Guid>();
			foreach (DataRow dr in dt.Rows)
			{
				foundIds.Add((string)dr["text_id"], (Guid)dr["id"]);
				input[(string)dr["text_id"]] = (Guid)dr["id"];
			}

			//find records not found in database table
			Dictionary<string, Guid> notFoundIds = new Dictionary<string, Guid>();
			foreach (var key in input.Keys)
			{
				if (foundIds.ContainsKey(key))
					continue;
				if (notFoundIds.ContainsKey(key))
					continue;
				notFoundIds.Add(key, Guid.NewGuid());
			}

			if (notFoundIds.Count > 0)
			{
				//create new parameters used for INSERT UNNEST operation
				parameterIds = new NpgsqlParameter("@id", NpgsqlDbType.Array | NpgsqlDbType.Uuid);
				parameterIds.Value = new List<Guid>();

				parameterTextIds = new NpgsqlParameter("@text_id", NpgsqlDbType.Array | NpgsqlDbType.Text);
				parameterTextIds.Value = new List<string>();

				//fill data in parameters with not found data
				foreach (var key in notFoundIds.Keys)
				{
					((List<Guid>)parameterIds.Value).Add(notFoundIds[key]);
					((List<string>)parameterTextIds.Value).Add(key);
				}

				//ignore conflicts because during operation someone else may create same id-text_id combination
				sql = $"INSERT INTO id_dict ( id, text_id ) SELECT * FROM UNNEST ( @id, @text_id ) ON CONFLICT DO NOTHING";
				_dbService.ExecuteSqlNonQueryCommand(sql, parameterIds, parameterTextIds);


				parameterTextIds = new NpgsqlParameter("@text_id", NpgsqlDbType.Array | NpgsqlDbType.Text);
				parameterTextIds.Value = new List<string>();

				//prepare new parameter
				foreach (var key in notFoundIds.Keys)
					((List<string>)parameterTextIds.Value).Add(key);

				//select newly created records and fill input
				sql = "SELECT * FROM id_dict WHERE text_id = ANY(@text_id)";
				dt = _dbService.ExecuteSqlQueryCommand(sql, parameterTextIds);

				foreach (DataRow dr in dt.Rows)
				{
					var textId = (string)dr["text_id"];
					var id = (Guid)dr["id"];

					input[textId] = id;
				}
			}
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

			return string.Join(Constants.SHARED_KEY_SEPARATOR, keys) ?? string.Empty;
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
}
