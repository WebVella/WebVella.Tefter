﻿namespace WebVella.Tefter.Assets.Migrations;


[TfApplicationMigration(AssetsConstants.ASSETS_APP_ID_STRING, "2024.12.5.1")]
public class AssetsMigration2024120501 : ITfApplicationMigration
{
	/// <summary>
	/// Called during the schema altering phase
	/// </summary>
	public Task MigrateStructureAsync(ITfApplication app, TfDatabaseBuilder dbBuilder)
	{
		dbBuilder
			.WithTableBuilder("assets_folder")
			.WithColumns(columns =>
			{
				columns.WithShortTextColumn("shared_key", c => c.Nullable());
			});
		return Task.CompletedTask;
	}
	/// <summary>
	/// Called during the data manipulation phase after the schema changes are completed
	/// </summary>
	/// <param name="app"></param>
	/// <param name="serviceprovider"></param>
	/// <param name="dbService"></param>
	/// <returns></returns>
	public Task MigrateDataAsync(ITfApplication app, IServiceProvider serviceprovider, ITfDatabaseService dbService)
	{
		return Task.CompletedTask;
	}
}
