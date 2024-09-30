namespace WebVella.Tefter.Migrations;

[TfSystemMigration("2024.7.24.1")]
internal class TefterSystemMigration2024072401 : TfSystemMigration
{
	public override void MigrateStructure(DatabaseBuilder dbBuilder)
	{
		#region TABLE: DATA_PROVIDER_COLUMN - make source_name and  source_type nullable

		dbBuilder
			.WithTableBuilder("data_provider_column")
			.WithColumns(columns =>
			{
				columns
					.WithTextColumn("source_name", c => { c.Nullable(); })
					.WithTextColumn("source_type", c => { c.Nullable(); });
			});
			
		#endregion
	}
}