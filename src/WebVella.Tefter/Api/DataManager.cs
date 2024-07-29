namespace WebVella.Tefter;

public partial interface IDataManager
{
}

public partial class DataManager : IDataManager
{
	private readonly IDatabaseService _dbService;

	public DataManager(
		IDatabaseService dbService)
	{
		_dbService = dbService;
	}
} 