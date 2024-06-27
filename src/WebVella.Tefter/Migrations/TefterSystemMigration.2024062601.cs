//namespace WebVella.Tefter.Migrations;

//[TefterSystemMigration("2024.6.26.1")]
//internal class TefterSystemMigration2024062601 : TefterSystemMigration
//{
//	public override void MigrateStructure(DatabaseBuilder dbBuilder)
//	{
//		// TABLE: DATA_PROVIDER_COLUMN
//		// ADD default value column
//		dbBuilder
//			.WithTableBuilder("data_provider_column")
//			.WithColumns(columns =>
//			{
//				columns
//					.AddTextColumn("default_value", c => { c.Nullable(); });
//			});
//	}
//}
