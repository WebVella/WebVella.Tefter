//namespace WebVella.Tefter.Migrations;

//[TfSystemMigration("2024.8.27.1")]
//internal class TefterSystemMigration2024082701 : TfSystemMigration
//{
//	public override void MigrateStructure(TfDatabaseBuilder dbBuilder)
//	{
//		#region  TABLE: space_view

//		dbBuilder
//			.NewTableBuilder(Guid.NewGuid(), "space_view")
//			.WithColumns(columns =>
//			{
//				columns
//					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
//					.AddShortTextColumn("name", c => { c.NotNullable(); })
//					.AddShortIntegerColumn("type", c => { c.NotNullable(); })
//					.AddShortIntegerColumn("position", c => { c.NotNullable(); })
//					.AddGuidColumn("space_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
//					.AddGuidColumn("space_data_id", c => { c.WithoutAutoDefaultValue().NotNullable(); });
//			})
//			.WithConstraints(constraints =>
//			{
//				constraints
//					.AddPrimaryKeyConstraint("pk_space_view_id", c => { c.WithColumns("id"); });
//			})
//			.WithIndexes(indexes =>
//			{
//				indexes
//					.AddBTreeIndex("ix_space_view_id", i => { i.WithColumns("id"); })
//					.AddBTreeIndex("ix_space_view_name", i => { i.WithColumns("name"); });
//			});

//		#endregion

//	}
//}