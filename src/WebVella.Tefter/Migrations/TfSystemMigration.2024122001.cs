//namespace WebVella.Tefter.Migrations;

//[TfSystemMigration("2024.12.20.1")]
//internal class TefterSystemMigration2024122001 : TfSystemMigration
//{
//	public override void MigrateStructure(TfDatabaseBuilder dbBuilder)
//	{
//		dbBuilder
//			.WithTableBuilder("template")
//			.WithColumns(columns =>
//			{
//				columns
//					.Remove("used_columns_json")
//					.AddTextColumn("space_data_json", c => { c.NotNullable().WithDefaultValue("[]"); });
//			});
//	}
//}