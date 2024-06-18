namespace WebVella.Tefter.Migrations;

internal partial interface IMigrationManager
{
	Task CheckExecutePendingMigrationsAsync();
}

internal partial class MigrationManager : IMigrationManager
{
	private readonly IServiceProvider _serviceProvider;
	private readonly IDatabaseManager _dbManager;
	private readonly IDboManager _dboManager;
	private readonly IDatabaseService _dbService;

	public MigrationManager(
		IServiceProvider serviceProvider,
		IDatabaseService dbService,
		IDatabaseManager databaseManager,
		IDboManager dboManager)
	{
		_serviceProvider = serviceProvider;
		_dboManager = dboManager;
		_dbManager = databaseManager;
		_dbService = dbService;
	}

	public async Task CheckExecutePendingMigrationsAsync()
	{
		using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
		{
			try
			{
				//checks and creates required postgres database extensions
				_dbService.ExecuteSqlNonQueryCommand(DatabaseSqlProvider.GenerateCreateExtensionsScript());

				var dbBuilder = _dbManager.GetDatabaseBuilder();

				//this is the case when database is empty and initial system structure should be created
				//this should be done by first (by version) system migration
				if (!dbBuilder.Build().Any())
				{
					//static migrations lists are already ordered by version
					var initialSystemMigration = _systemMigrations.First();
					initialSystemMigration.Instance.MigrateStructure(dbBuilder);

					//if save fails new DatabaseUpdateException with details will be thrown by save method itself
					_dbManager.SaveChanges(dbBuilder);

					//execute data migration method
					await initialSystemMigration.Instance.MigrateDataAsync(_serviceProvider);

					bool success = await _dboManager.InsertAsync<Migration>(new Migration
					{
						Id = Guid.NewGuid(),
						MigrationClassName = initialSystemMigration.MigrationClassName,
						AddOnId = null,
						ExecutedOn = DateTime.Now,
						MajorVer = initialSystemMigration.Version.Major,
						MinorVer = initialSystemMigration.Version.Minor,
						BuildVer = initialSystemMigration.Version.Build,
						RevisionVer = initialSystemMigration.Version.Revision
					});
					if (!success)
						throw new DatabaseException("Failed to save migration record.");


				}

				var allExecutedMigrations = (await _dboManager.GetListAsync<Migration>()).OrderBy(x => x.Version).ToList();
				var systemExecutedMigrations = allExecutedMigrations.Where(x => x.AddOnId is null);
				var addonsExecutedMigrations = allExecutedMigrations.Where(x => x.AddOnId is not null);

				var lastExecutedSystemMigrationVersion = systemExecutedMigrations.Last().Version;
				foreach (var migration in _systemMigrations.OrderBy(x => x.Version))
				{
					var isExecuted = systemExecutedMigrations.Any(x => x.Version == migration.Version);
					if (isExecuted)
						continue;

					if (lastExecutedSystemMigrationVersion > migration.Version)
						throw new DatabaseException($"System migration with version [{migration.Version}] " +
							$" is pending for execution, but greater version migration " +
							$"[{lastExecutedSystemMigrationVersion}] is already executed!");


					migration.Instance.MigrateStructure(dbBuilder);

					//if save fails new DatabaseUpdateException with details will be thrown by save method itself
					_dbManager.SaveChanges(dbBuilder);

					//execute data migration method
					await migration.Instance.MigrateDataAsync(_serviceProvider);

					bool success = await _dboManager.InsertAsync<Migration>(new Migration
					{
						Id = Guid.NewGuid(),
						MigrationClassName = migration.MigrationClassName,
						AddOnId = null,
						ExecutedOn = DateTime.Now,
						MajorVer = migration.Version.Major,
						MinorVer = migration.Version.Minor,
						BuildVer = migration.Version.Build,
						RevisionVer = migration.Version.Revision
					});
					if (!success)
						throw new DatabaseException("Failed to save migration record.");
				}


				//TODO: at this moment we will process only system migrations 

				scope.Complete();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}
