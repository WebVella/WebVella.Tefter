namespace WebVella.Tefter.Migrations;

[TefterSystemMigration("2024.6.26.2")]
internal class TefterSystemMigration2024062602 : TefterSystemMigration
{
	public override void MigrateStructure(DatabaseBuilder dbBuilder)
	{
		// TABLE: DATA_PROVIDER_COLUMN
		dbBuilder
			.WithTableBuilder("data_provider_column")
			.WithColumns(columns =>
			{
				columns
					.Remove("is_row_key")
					.AddBooleanColumn("is_unique", c => { c.WithDefaultValue(false).NotNullable(); })
					.AddBooleanColumn("is_part_of_composite_key", c => { c.WithDefaultValue(false).NotNullable(); });
					
			});
	}
}
