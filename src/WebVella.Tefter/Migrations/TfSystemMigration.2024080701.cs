//namespace WebVella.Tefter.Migrations;

//[TfSystemMigration("2024.8.7.1")]
//internal class TefterSystemMigration2024080701 : TfSystemMigration
//{
//	public override void MigrateStructure(TfDatabaseBuilder dbBuilder)
//	{
//		#region  TABLE: space

//		dbBuilder
//			.NewTableBuilder(Guid.NewGuid(), "space")
//			.WithColumns(columns =>
//			{
//				columns
//					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
//					.AddShortTextColumn("name", c => { c.NotNullable(); })
//					.AddShortIntegerColumn("position", c => { c.NotNullable(); })
//					.AddBooleanColumn("is_private", c => { c.NotNullable().WithDefaultValue(false); })
//					.AddShortTextColumn("icon", c => { c.Nullable(); })
//					.AddShortIntegerColumn("color", c => { c.Nullable(); });
//			})
//			.WithConstraints(constraints =>
//			{
//				constraints
//					.AddPrimaryKeyConstraint("pk_space_id", c => { c.WithColumns("id"); });
//			})
//			.WithIndexes(indexes =>
//			{
//				indexes
//					.AddBTreeIndex("ix_space_id", i => { i.WithColumns("id"); })
//					.AddBTreeIndex("ix_space_name", i => { i.WithColumns("name"); });
//			});

//		#endregion

//		#region  TABLE: space_data

//		dbBuilder
//			.NewTableBuilder(Guid.NewGuid(), "space_data")
//			.WithColumns(columns =>
//			{
//				columns
//					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
//					.AddGuidColumn("space_id", c => { c.NotNullable(); })
//					.AddShortTextColumn("name", c => { c.NotNullable(); })
//					.AddShortIntegerColumn("position", c => { c.NotNullable(); })
//					.AddTextColumn("filters_json", c => { c.NotNullable().WithDefaultValue("[]"); });
//			})
//			.WithConstraints(constraints =>
//			{
//				constraints
//					.AddPrimaryKeyConstraint("pk_space_data_id", c => { c.WithColumns("id"); })
//					.AddForeignKeyConstraint("fk_space_data_space", c =>
//					{
//						c
//						.WithForeignTable("space")
//						.WithForeignColumns("id")
//						.WithColumns("space_id");
//					});
//			})
//			.WithIndexes(indexes =>
//			{
//				indexes
//					.AddBTreeIndex("ix_space_data_id", i => { i.WithColumns("id"); })
//					.AddBTreeIndex("ix_space_data_space_id", i => { i.WithColumns("space_id"); })
//					.AddBTreeIndex("ix_space_data_name", i => { i.WithColumns("name"); });
//			});

//		#endregion

//	}
//}