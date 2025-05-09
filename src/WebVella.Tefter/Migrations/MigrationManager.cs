namespace WebVella.Tefter.Migrations;

internal partial interface IMigrationManager
{
	Task CheckExecutePendingMigrationsAsync();
}

internal partial class MigrationManager : IMigrationManager
{
	private readonly IServiceProvider _serviceProvider;
	private readonly ITfDatabaseManager _dbManager;
	private readonly ITfDboManager _dboManager;
	private readonly ITfDatabaseService _dbService;
	private readonly ITfMetaService _metaService;

	public MigrationManager(
		IServiceProvider serviceProvider,
		ITfDatabaseService dbService,
		ITfDatabaseManager databaseManager,
		ITfDboManager dboManager,
		ITfMetaService metaService)
	{
		_serviceProvider = serviceProvider;
		_dboManager = dboManager;
		_dbManager = databaseManager;
		_dbService = dbService;
		_metaService = metaService;
	}

	public async Task CheckExecutePendingMigrationsAsync()
	{
		using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
		{
			await CheckExecutePendingSystemMigrationsAsync();

			await CheckExecutePendingApplicationMigrationsAsync();

			scope.Complete();
		}
	}

	public async Task CheckExecutePendingSystemMigrationsAsync()
	{
		//checks and creates required postgres database extensions
		_dbService.ExecuteSqlNonQueryCommand(TfDatabaseSqlProvider.GenerateSystemRequirementsScript());

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
				ApplicationId = null,
				ExecutedOn = DateTime.Now,
				MajorVer = initialSystemMigration.Version.Major,
				MinorVer = initialSystemMigration.Version.Minor,
				BuildVer = initialSystemMigration.Version.Build,
				RevisionVer = initialSystemMigration.Version.Revision
			});
			if (!success)
				throw new TfDatabaseException("Failed to save migration record.");


		}

		var allExecutedMigrations = (await _dboManager.GetListAsync<Migration>()).OrderBy(x => x.Version).ToList();
		var systemExecutedMigrations = allExecutedMigrations.Where(x => x.ApplicationId is null);
		var addonsExecutedMigrations = allExecutedMigrations.Where(x => x.ApplicationId is not null);

		var lastExecutedSystemMigrationVersion = systemExecutedMigrations.Last().Version;
		foreach (var migration in _systemMigrations.OrderBy(x => x.Version))
		{
			var isExecuted = systemExecutedMigrations.Any(x => x.Version == migration.Version);
			if (isExecuted)
				continue;

			if (lastExecutedSystemMigrationVersion > migration.Version)
				throw new TfDatabaseException($"System migration with version [{migration.Version}] " +
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
				ApplicationId = null,
				ExecutedOn = DateTime.Now,
				MajorVer = migration.Version.Major,
				MinorVer = migration.Version.Minor,
				BuildVer = migration.Version.Build,
				RevisionVer = migration.Version.Revision
			});
			if (!success)
				throw new TfDatabaseException("Failed to save migration record.");
		}
	}

	public async Task CheckExecutePendingApplicationMigrationsAsync()
	{
		foreach (var migration in _applicationMigrations.OrderBy(x => x.Version))
		{
			var allInstalledApplicationMigrations = (await _dboManager.GetListAsync<Migration>())
				.Where(x => x.ApplicationId is not null)
				.OrderBy(x => x.Version).ToList();

			var isExecuted = allInstalledApplicationMigrations.Any(x =>
						x.ApplicationId == migration.ApplicationId &&
						x.Version == migration.Version);

			if (isExecuted)
				continue;


			ITfApplicationAddon tfApp = _metaService.GetApplication(migration.ApplicationId);

			if (tfApp is null)
			{
				throw new Exception($"Application migration for application with id={migration.ApplicationId} " +
					$" and version [{migration.Version}] cannot be executed " +
					$"because application with such id is not found.");
			}


			var lastExecutedApplicationMigration = allInstalledApplicationMigrations
				.Where(x => x.ApplicationId == migration.ApplicationId)
				.OrderBy(x => x.Version)
				.LastOrDefault();


			if (lastExecutedApplicationMigration is not null &&
				lastExecutedApplicationMigration.Version > migration.Version)
			{
				throw new TfDatabaseException($"Application migration for application {tfApp.AddonName}({tfApp.AddonId}) " +
					$" with version [{migration.Version}] " +
					$" is pending for execution, but greater version migration " +
					$"[{lastExecutedApplicationMigration.Version}] is already executed!");
			}

			var dbBuilder = _dbManager.GetDatabaseBuilder();


			await migration.Instance.MigrateStructureAsync(tfApp, dbBuilder);

			//if save fails new DatabaseUpdateException with details will be thrown by save method itself
			_dbManager.SaveChanges(dbBuilder);

			//execute data migration method
			await migration.Instance.MigrateDataAsync(tfApp, _serviceProvider, _dbService);

			bool success = await _dboManager.InsertAsync<Migration>(new Migration
			{
				Id = Guid.NewGuid(),
				MigrationClassName = migration.MigrationClassName,
				ApplicationId = migration.ApplicationId,
				ExecutedOn = DateTime.Now,
				MajorVer = migration.Version.Major,
				MinorVer = migration.Version.Minor,
				BuildVer = migration.Version.Build,
				RevisionVer = migration.Version.Revision
			});
			if (!success)
				throw new TfDatabaseException("Failed to save application migration record.");
		}

	}
}
