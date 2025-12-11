using Microsoft.Extensions.DependencyInjection;

namespace WebVella.Tefter.Assets.Migrations;


[TfApplicationMigration(AssetsApp.ID, "2024.11.1.1")]
public class AssetsMigration2024110101 : ITfApplicationMigration
{
    public async Task MigrateDataAsync(ITfApplicationAddon app, IServiceProvider serviceProvider, ITfDatabaseService dbService)
    {
        await Task.Delay(0);
        var counterColumn = $"{TfConstants.TF_SHARED_COLUMN_PREFIX}{TfConstants.TEFTER_DEFAULT_OBJECT_NAME}_assets_counter";
        ITfService tfService = serviceProvider.GetService<ITfService>()!;
        _ = tfService.CreateSharedColumn(
                new TfSharedColumn
                {
                    Id = new AssetsApp().AddonId,
                    DataIdentity = TfConstants.TEFTER_DEFAULT_OBJECT_NAME,
                    DbName = counterColumn,
                    DbType = TfDatabaseColumnType.LongInteger,
                    IncludeInTableSearch = false
                }
            );

        IAssetsService assetService = serviceProvider.GetService<IAssetsService>()!;
        _ = assetService.CreateFolder(new AssetsFolder
        {
            Id = new AssetsApp().AddonId,
            Name = TfConstants.TEFTER_DEFAULT_OBJECT_NAME,
            DataIdentity = TfConstants.TEFTER_DEFAULT_OBJECT_NAME,
            CountSharedColumnName = counterColumn
        });

    }

    public async Task MigrateStructureAsync(ITfApplicationAddon app, TfDatabaseBuilder dbBuilder)
    {
        await Task.Delay(0);

        dbBuilder
            .NewTableBuilder(Guid.NewGuid(), "assets_folder")
            .WithColumns(columns =>
            {
                columns
                    .AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
                    .AddShortTextColumn("name", c => { c.NotNullable(); })
                    .AddShortTextColumn("data_identity", c => { c.Nullable(); })
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
                    .AddShortTextColumn("identity_row_id", c => { c.AsSha1ExpressionFromColumns("id"); })
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
        
        #region  TABLE: Thread_TAG

        dbBuilder
            .NewTableBuilder(Guid.NewGuid(), "assets_asset_tag")
            .WithColumns(columns =>
            {
                columns
                    .AddGuidColumn("assets_asset_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
                    .AddGuidColumn("tag_id", c => { c.WithoutAutoDefaultValue().NotNullable(); });
            })
            .WithConstraints(constraints =>
            {
                constraints
                    .AddPrimaryKeyConstraint("pk_assets_asset_tags", c => { c.WithColumns("assets_asset_id", "tag_id"); })
                    .AddForeignKeyConstraint("fk_assets_asset_tags_assets_asset", c =>
                    {
                        c.WithForeignTable("assets_asset")
                            .WithForeignColumns("id")
                            .WithColumns("assets_asset_id");
                    })
                    .AddForeignKeyConstraint("fk_assets_asset_tags_tag", c =>
                    {
                        c.WithForeignTable("tf_tag")
                            .WithForeignColumns("id")
                            .WithColumns("tag_id");
                    });
            })
            .WithIndexes(indexes =>
            {
                indexes
                    .AddBTreeIndex("ix_assets_asset_tags_assets_asset_id", i => { i.WithColumns("assets_asset_id"); })
                    .AddBTreeIndex("ix_assets_asset_tags_tag_id", i => { i.WithColumns("tag_id"); });
            });

        #endregion        
    }
}
