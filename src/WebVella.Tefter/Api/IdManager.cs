namespace WebVella.Tefter;

internal interface IIdManager
{
	/// <summary>
	/// Gets unique identifier for specified text
	/// </summary>
	/// <param name="textId"></param>
	/// <returns></returns>
	Result<Guid> Get(
		string textId);

	/// <summary>
	/// Gets async unique identifier for specified text
	/// </summary>
	/// <param name="textId"></param>
	/// <returns></returns>
	Task<Result<Guid>> GetAsync(
		string textId);

	/// <summary>
	/// Gets unique identifier for provider argument. Argument is converted to text and result is for it.
	/// </summary>
	/// <param name="guidId"></param>
	/// <returns></returns>
	Result<Guid> Get(
		Guid guidId);

	/// <summary>
	/// Gets async unique identifier for provider argument. Argument is converted to text and result is for it.
	/// </summary>
	/// <param name="guidId"></param>
	/// <returns></returns>
	Task<Result<Guid>> GetAsync(
		Guid cuidId);
}

internal class IdManager : IIdManager
{
	private const string SQL = "SELECT * FROM _tefter_id_dict_insert_select( @text_id, @id )";
	private readonly IDatabaseService _dbService;

	public IdManager(
		IDatabaseService dbService)
	{
		_dbService = dbService;
	}


	/// <summary>
	/// Gets unique identifier for specified text
	/// </summary>
	/// <param name="textId"></param>
	/// <returns></returns>
	public Result<Guid> Get(
		string textId)
	{
		try
		{
			Guid id = Guid.Empty;
			using var sqlReader = _dbService.GetReader(SQL,
				new NpgsqlParameter("text_id", textId ?? string.Empty),
				new NpgsqlParameter("id", DBNull.Value));

			sqlReader.Read();

			id = sqlReader.GetGuid(0);

			sqlReader.Close();

			return Result.Ok(id);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get GUID for specified text identifier.").CausedBy(ex));
		}
	}

	/// <summary>
	/// Gets async unique identifier for specified text
	/// </summary>
	/// <param name="textId"></param>
	/// <returns></returns>
	public async Task<Result<Guid>> GetAsync(
		string textId)
	{
		try
		{
			Guid id = Guid.Empty;
			using var sqlReader = _dbService.GetReader(SQL,
				new NpgsqlParameter("text_id", textId ?? string.Empty),
				new NpgsqlParameter("id", DBNull.Value));

			sqlReader.Read();

			id = sqlReader.GetGuid(0);

			sqlReader.Close();

			return Result.Ok(id);
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
	public Result<Guid> Get(
		Guid guidId)
	{
		try
		{
			Guid id = Guid.Empty;
			using var sqlReader = _dbService.GetReader(SQL,
				new NpgsqlParameter("text_id", guidId.ToString()),
				new NpgsqlParameter("id", guidId));

			sqlReader.Read();

			id = sqlReader.GetGuid(0);

			sqlReader.Close();

			return Result.Ok(id);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get GUID for specified text identifier.").CausedBy(ex));
		}
	}

	/// <summary>
	/// Gets async unique identifier for provider argument. Argument is converted to text and result is for it.
	/// </summary>
	/// <param name="guidId"></param>
	/// <returns></returns>
	public async Task<Result<Guid>> GetAsync(
		Guid guidId)
	{
		try
		{
			Guid id = Guid.Empty;
			using var sqlReader = await _dbService.GetReaderAsync(SQL,
				new NpgsqlParameter("text_id", guidId.ToString()),
				new NpgsqlParameter("id", guidId));

			sqlReader.Read();

			id = sqlReader.GetGuid(0);

			sqlReader.Close();

			return Result.Ok(id);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get GUID for specified text identifier.").CausedBy(ex));
		}
	}
}
