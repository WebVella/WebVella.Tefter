namespace WebVella.Tefter.Migrations;

public interface ITfApplicationMigration
{
	public Task MigrateStructureAsync(
		TfApplicationBase app,
		DatabaseBuilder db);

	public Task MigrateDataAsync(
		TfApplicationBase app,
		IServiceProvider serviceprovider,
		IDatabaseService dbService);
}
