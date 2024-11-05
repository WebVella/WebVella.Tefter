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
}

public partial class TfDataManager
{
	private const string SHARED_KEY_SEPARATOR = "$$$";
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
			var dt = _dbService.ExecuteSqlQueryCommand(SQL,
				new NpgsqlParameter("text_id", CombineKey(textId)),
				new NpgsqlParameter("id", DBNull.Value));

			return Result.Ok((Guid)dt.Rows[0][0]);
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
		try
		{
			var dt = _dbService.ExecuteSqlQueryCommand(SQL,
				new NpgsqlParameter("text_id", guidId.ToString()),
				new NpgsqlParameter("id", guidId));

			return Result.Ok((Guid)dt.Rows[0][0]);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get GUID for specified text identifier.").CausedBy(ex));
		}
	}


	private string CombineKey(params string[] keys)
	{
		if (keys == null || keys.Length == 0)
			return string.Empty;

		return string.Join(SHARED_KEY_SEPARATOR, keys) ?? string.Empty;
	}
}
