namespace WebVella.Tefter.Migrations;

[TfSystemMigration("2025.3.13.1")]
internal class TefterSystemMigration2025031301 : TfSystemMigration
{
	public override void MigrateStructure(TfDatabaseBuilder dbBuilder)
	{
		dbBuilder
			.WithTableBuilder("id_dict")
			.WithConstraints(constraints =>
			{
				constraints
					.AddUniqueKeyConstraint("ux_id_dict_text_id_ux", c => { c.WithColumns("text_id"); });
			});
	}
}