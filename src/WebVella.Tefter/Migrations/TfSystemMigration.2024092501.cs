namespace WebVella.Tefter.Migrations;

[TfSystemMigration("2024.9.25.1")]
internal class TefterSystemMigration2024092501 : TfSystemMigration
{
	public override void MigrateStructure(DatabaseBuilder dbBuilder)
	{
		#region  TABLE: space_data

		dbBuilder
			.WithTableBuilder("space_view")
			.WithColumns(columns =>
			{
				columns
					.AddTextColumn("settings_json", c => { c.NotNullable().WithDefaultValue("{}"); });
			});
		#endregion

	}
}