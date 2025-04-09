//namespace WebVella.Tefter.Migrations;

//[TfSystemMigration("2024.10.28.1")]
//internal class TefterSystemMigration2024102801 : TfSystemMigration
//{
//	public override void MigrateStructure(TfDatabaseBuilder dbBuilder)
//	{
//		#region  TABLE: files

//		dbBuilder
//			.NewTableBuilder(Guid.NewGuid(), "files")
//			.WithColumns(columns =>
//			{
//				columns
//					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
//					.AddTextColumn("filepath", c => { c.NotNullable(); })
//					.AddGuidColumn("created_by", c => { c.Nullable(); })
//					.AddDateTimeColumn("created_on", c => { c.NotNullable().WithAutoDefaultValue(); })
//					.AddGuidColumn("last_modified_by", c => { c.Nullable(); })
//					.AddDateTimeColumn("last_modified_on", c => { c.NotNullable().WithAutoDefaultValue(); });

//			})
//			.WithConstraints(constraints =>
//			{
//				constraints
//					.AddPrimaryKeyConstraint("pk_files_id", c => { c.WithColumns("id"); });
//			})
//			.WithIndexes(indexes =>
//			{
//				indexes
//					.AddBTreeIndex("ix_files_id", i => { i.WithColumns("id"); })
//					.AddHashIndex("ix_files_filepath", i => { i.WithColumn("filepath"); });
//			});

//		#endregion
//	}
//}