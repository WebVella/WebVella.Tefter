using FluentResults;

namespace WebVella.Tefter.Migrations;

[TefterSystemMigration("2024.6.24.1")]
internal class TefterSystemMigration2024062401 : TefterSystemMigration
{
	public override void MigrateStructure(DatabaseBuilder dbBuilder)
	{
		// TABLE: DATA_PROVIDER
		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "data_provider")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddTextColumn("name", c => { c.NotNullable(); })
					.AddAutoIncrementColumn("index")
					.AddGuidColumn("type_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
					.AddTextColumn("type_name", c => { c.NotNullable(); })
					.AddTextColumn("composite_key_prefix", c => { c.Nullable(); })
					.AddTextColumn("settings_json", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_data_provider_id", c => { c.WithColumns("id"); })
					.AddUniqueKeyConstraint("ux_data_provider_index", c => { c.WithColumns("index"); });
			});

		// TABLE: DATA_PROVIDER_COLUMN
		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "data_provider_column")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddAutoIncrementColumn("index")
					.AddGuidColumn("data_provider_id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddBooleanColumn("is_row_key", c => { c.NotNullable().WithDefaultValue(false); })
					.AddTextColumn("source_name", c => { c.NotNullable(); })
					.AddTextColumn("source_type", c => { c.NotNullable(); })
					.AddTextColumn("db_name", c => { c.NotNullable(); })
					.AddNumberColumn("db_type", c => { c.NotNullable(); })
					.AddBooleanColumn("auto_default_value", c => { c.NotNullable().WithDefaultValue(false); })
					.AddBooleanColumn("is_nullable", c => { c.NotNullable().WithDefaultValue(false); })
					.AddBooleanColumn("is_sortable", c => { c.NotNullable().WithDefaultValue(false); })
					.AddBooleanColumn("is_searchable", c => { c.NotNullable().WithDefaultValue(false); })
					.AddNumberColumn("preferred_search_type", c => { c.NotNullable().WithDefaultValue(0); })
					.AddBooleanColumn("include_table_search", c => { c.NotNullable().WithDefaultValue(false); })
					.AddBooleanColumn("is_system", c => { c.NotNullable().WithDefaultValue(false); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_data_provider_column_id", c => { c.WithColumns("id"); })
					.AddUniqueKeyConstraint("ux_data_provider_column_index", c => { c.WithColumns("index"); })
					.AddForeignKeyConstraint("fk_data_provider_data_provider_column", c => 
					{
						c
						.WithColumns("data_provider_id")
						.WithForeignTable("data_provider")
						.WithForeignColumns("id");
					});
			});
	}
}
