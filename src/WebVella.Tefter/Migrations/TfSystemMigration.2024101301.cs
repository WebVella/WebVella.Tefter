namespace WebVella.Tefter.Migrations;

[TfSystemMigration("2024.10.13.1")]
internal class TefterSystemMigration2024101301 : TfSystemMigration
{
	public override void MigrateStructure(TfDatabaseBuilder dbBuilder)
	{
		#region  TABLE: space_data

		dbBuilder
			.WithTableBuilder("space_view")
			.WithColumns(columns =>
			{
				columns
					.AddTextColumn("presets_json", c => { c.NotNullable().WithDefaultValue("[]"); })
					.AddTextColumn("groups_json", c => { c.NotNullable().WithDefaultValue("[]"); });
			});
		#endregion

	}
}