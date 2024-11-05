namespace WebVella.Tefter.Migrations;

public interface ITfApplicationMigration
{
	public Task MigrateStructureAsync(
		ITfApplication app,
		TfDatabaseBuilder db);

	public Task MigrateDataAsync(
		ITfApplication app,
		IServiceProvider serviceprovider,
		ITfDatabaseService dbService);
}
