namespace WebVella.Tefter.Migrations;

[TfSystemMigration("2024.8.19.1")]
internal class TefterSystemMigration2024081901 : TfSystemMigration
{
	public override void MigrateStructure(TfDatabaseBuilder dbBuilder)
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