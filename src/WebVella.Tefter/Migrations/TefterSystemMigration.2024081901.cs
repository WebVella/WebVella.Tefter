namespace WebVella.Tefter.Migrations;

[TefterSystemMigration("2024.8.19.1")]
internal class TefterSystemMigration2024081901 : TefterSystemMigration
{
	public override void MigrateStructure(DatabaseBuilder dbBuilder)
	{
		#region  TABLE: space_data

		dbBuilder
			.WithTableBuilder("space_data")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("data_provider_id", c => { c.WithoutAutoDefaultValue().NotNullable(); });
			});
		#endregion

	}
}