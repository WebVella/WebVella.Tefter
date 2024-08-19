namespace WebVella.Tefter.Migrations;

[TefterSystemMigration("2024.8.16.1")]
internal class TefterSystemMigration2024081601 : TefterSystemMigration
{
	public override void MigrateStructure(DatabaseBuilder dbBuilder)
	{
		#region  TABLE: space_data

		dbBuilder
			.WithTableBuilder("space_data")
			.WithColumns(columns =>
			{
				columns
					.AddTextColumn("columns_json", c => { c.NotNullable().WithDefaultValue("[]"); });
			});
		#endregion

	}
}