//namespace WebVella.Tefter.Migrations;

//[TfSystemMigration("2024.9.10.1")]
//internal class TefterSystemMigration2024091001 : TfSystemMigration
//{
//	public override void MigrateStructure(TfDatabaseBuilder dbBuilder)
//	{

//		#region  TABLE: space_data

//		dbBuilder
//			.WithTableBuilder("space_data")
//			.WithColumns(columns =>
//			{
//				columns
//					.AddTextColumn("sort_orders_json", c => { c.NotNullable().WithDefaultValue("[]"); });
//			});

//		#endregion

//		#region  TABLE: space_view_column

//		dbBuilder
//			.WithTableBuilder("space_view_column")
//			.WithColumns(columns =>
//			{
//				columns
//					.AddTextColumn("settings_json", c => { c.NotNullable().WithDefaultValue("{}"); });
//			});

//		#endregion

//	}
//}