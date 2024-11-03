namespace WebVella.Tefter.Assets.Migrations;


[TfApplicationMigration(TfAssetConstants.ASSETS_APP_ID_STRING, "2024.11.1.1")]
public class AssetsMigration2024110101 : ITfApplicationMigration
{
	public async Task MigrateDataAsync(ITfApplication app, IServiceProvider serviceprovider, IDatabaseService dbService)
	{
		await Task.Delay(0);
	}

	public async Task MigrateStructureAsync(ITfApplication app, DatabaseBuilder dbBuilder)
	{
		await Task.Delay(0);

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "assets_folder")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddShortTextColumn("name", c => { c.NotNullable(); })
					.AddShortTextColumn("shared_key", c => { c.NotNullable(); })
					.AddShortTextColumn("count_shared_column_name", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_assets_folder_id", c => { c.WithColumns("id"); });
			});

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "assets_asset")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddGuidColumn("folder_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
					.AddShortIntegerColumn("type", c => { c.NotNullable(); })
					.AddTextColumn("content_json", c => { c.NotNullable(); })
					.AddTextColumn("x_search", c => { c.NotNullable(); })
					.AddGuidColumn("created_by", c => { c.WithoutAutoDefaultValue().NotNullable(); })
					.AddDateTimeColumn("created_on", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddGuidColumn("modified_by", c => { c.WithoutAutoDefaultValue().NotNullable(); })
					.AddDateTimeColumn("modified_on", c => { c.WithAutoDefaultValue().NotNullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_assets_asset_id", c => { c.WithColumns("id"); })
					.AddForeignKeyConstraint("fk_assets_asset_folder", c =>
					{
						c
						.WithForeignTable("assets_folder")
						.WithForeignColumns("id")
						.WithColumns("folder_id");
					});
			});


		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "assets_related_sk")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
					.AddGuidColumn("asset_id", c => { c.WithoutAutoDefaultValue().NotNullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_assets_related_sk", c => { c.WithColumns("id", "asset_id"); })
					.AddForeignKeyConstraint("fk_assets_related_sk_asset", c =>
					{
						c
						.WithForeignTable("assets_asset")
						.WithForeignColumns("id")
						.WithColumns("asset_id");
					});
			});


	}
}
