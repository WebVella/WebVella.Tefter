namespace WebVella.Tefter.Migrations;

[TefterSystemMigration("2024.6.28.1")]
internal class TefterSystemMigration2024062801 : TefterSystemMigration
{
	public override void MigrateStructure(DatabaseBuilder dbBuilder)
	{
		#region  TABLE: DATA_PROVIDER_SHARED_KEY
		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "data_provider_shared_key")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.NotNullable().WithAutoDefaultValue(); })
					.AddGuidColumn("data_provider_id", c => { c.NotNullable().WithoutAutoDefaultValue(); })
					.AddShortTextColumn("db_name", c => { c.NotNullable(); })
					.AddShortTextColumn("description", c => { c.NotNullable().WithDefaultValue(string.Empty); })
					.AddBooleanColumn("is_synchronization_key", c => { c.NotNullable().WithDefaultValue(false); })
					.AddTextColumn("column_ids_json", c => { c.NotNullable().WithDefaultValue("[]"); })
					.AddShortIntegerColumn("version", c => { c.NotNullable().WithDefaultValue(1); })
					.AddDateTimeColumn("last_modified_on", c => { c.NotNullable().WithAutoDefaultValue(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_data_provider_shared_key", c => { c.WithColumns("id"); })
					.AddForeignKeyConstraint("fk_data_provider_shared_key_data_provider", c =>
					{
						c.WithColumns("data_provider_id")
						.WithForeignTable("data_provider")
						.WithForeignColumns("id");
					});
			});

		#endregion
	}
}
