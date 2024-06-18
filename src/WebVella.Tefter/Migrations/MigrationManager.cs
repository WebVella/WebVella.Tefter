namespace WebVella.Tefter.Migrations;

internal partial interface IMigrationManager
{
	Task CheckExecutePendingMigrationsAsync();
}

internal partial class MigrationManager : IMigrationManager
{
	private readonly IDatabaseManager _dbManager;
	private readonly IDboManager _dboManager;
	private readonly IDatabaseService _dbService;

	public MigrationManager( IDatabaseService dbService, 
		IDatabaseManager databaseManager, IDboManager dboManager )
	{
		_dboManager = dboManager;
		_dbManager = databaseManager;
		_dbService = dbService;
	}

	public async Task CheckExecutePendingMigrationsAsync()
	{
		using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
		{
			var dbBuilder = _dbManager.GetDatabaseBuilder();
			var existingTables = dbBuilder.Build();

			//this is the case when database is empty and initial system structure should be created
			//this should be done by first (by version) system migration
			if (!existingTables.Any())
			{
				var initialSystemMigration = _systemMigrations.OrderBy(x => x.Version).First();
				initialSystemMigration.Instance.Migrate(_dbManager, _dboManager);

				var result = _dbManager.SaveChanges(dbBuilder);
				if(result.IsSuccess)
				{
					//_dboManager.InsertAsync<>
				}
			}
		}
	}
}
