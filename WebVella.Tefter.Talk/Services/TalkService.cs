namespace WebVella.Tefter.Talk.Services;

internal interface ITalkService
{
	public void InsertSampleData();
}


internal class TalkService : ITalkService
{
	public readonly IDatabaseService _dbService;

	public TalkService(IDatabaseService dbService)
	{
		_dbService = dbService;
	}

	public void InsertSampleData()
	{
		for (int i = 0; i < 10; i++)
		{
			_dbService.ExecuteSqlNonQueryCommand("INSERT INTO talk_sample_table VALUES(@id)",
				new Npgsql.NpgsqlParameter("@id", Guid.NewGuid()));
		}
	}
}
