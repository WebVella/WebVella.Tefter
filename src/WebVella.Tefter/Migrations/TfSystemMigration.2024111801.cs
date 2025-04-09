//namespace WebVella.Tefter.Migrations;

//[TfSystemMigration("2024.11.18.1")]
//internal class TefterSystemMigration2024111801 : TfSystemMigration
//{
//	public override void MigrateStructure(TfDatabaseBuilder dbBuilder)
//	{
//		dbBuilder
//			.WithTableBuilder("data_provider")
//			.WithColumns(columns =>
//			{
//				columns
//					.AddTextColumn("sync_primary_key_columns_json", c => { c.NotNullable().WithDefaultValue("[]"); });
//			});
//	}
//}