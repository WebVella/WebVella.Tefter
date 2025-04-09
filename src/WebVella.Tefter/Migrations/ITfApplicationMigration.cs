namespace WebVella.Tefter.Migrations;

public interface ITfApplicationMigration
{
	public Task MigrateStructureAsync(
		ITfApplicationAddon app,
		TfDatabaseBuilder db);

	public Task MigrateDataAsync(
		ITfApplicationAddon app,
		IServiceProvider serviceprovider,
		ITfDatabaseService dbService);
}
