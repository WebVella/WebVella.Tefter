namespace WebVella.Tefter.Migrations;

[TfSystemMigration("2024.11.28.1")]
internal class TefterSystemMigration2024112801 : TfSystemMigration
{
	public override void MigrateStructure(TfDatabaseBuilder dbBuilder)
	{
		dbBuilder
			.WithTableBuilder("space_view_column")
			.WithColumns(columns =>
			{
				columns
					.AddShortTextColumn("icon", c => { c.NotNullable().WithDefaultValue(""); })
					.AddBooleanColumn("only_icon", c => { c.NotNullable().WithDefaultValue(false); });
			});
	}
}