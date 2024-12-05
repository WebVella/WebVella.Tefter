namespace WebVella.Tefter.Assets.Migrations;


[TfApplicationMigration(TfAssetsConstants.ASSETS_APP_ID_STRING, "2024.12.5.1")]
public class AssetsMigration2024120501 : ITfApplicationMigration
{
	public async Task MigrateDataAsync(ITfApplication app, IServiceProvider serviceprovider, ITfDatabaseService dbService)
	{
		await Task.Delay(0);
	}

	public async Task MigrateStructureAsync(ITfApplication app, TfDatabaseBuilder dbBuilder)
	{
		await Task.Delay(0);

		dbBuilder
			.WithTableBuilder("assets_folder")
			.WithColumns(columns =>
			{
				columns.WithShortTextColumn("shared_key", c => c.Nullable());
			});
	}
}
