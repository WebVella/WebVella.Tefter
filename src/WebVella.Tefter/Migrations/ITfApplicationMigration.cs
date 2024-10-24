namespace WebVella.Tefter.Migrations;

public interface ITfApplicationMigration
{
	public Task MigrateStructureAsync(
		ITfApplication app,
		DatabaseBuilder db);

	public Task MigrateDataAsync(
		ITfApplication app,
		IServiceProvider serviceprovider,
		IDatabaseService dbService);
}
