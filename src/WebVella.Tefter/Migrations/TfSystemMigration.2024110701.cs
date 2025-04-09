//namespace WebVella.Tefter.Migrations;

//[TfSystemMigration("2024.11.7.1")]
//internal class TefterSystemMigration2024110701 : TfSystemMigration
//{
//	public override void MigrateStructure(TfDatabaseBuilder dbBuilder)
//	{
//		#region  TABLE: repository_file

//		dbBuilder
//			.NewTableBuilder(Guid.NewGuid(), "repository_file")
//			.WithColumns(columns =>
//			{
//				columns
//					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
//					.AddTextColumn("filename", c => { c.NotNullable(); })
//					.AddGuidColumn("created_by", c => { c.Nullable(); })
//					.AddDateTimeColumn("created_on", c => { c.NotNullable().WithAutoDefaultValue(); })
//					.AddGuidColumn("last_modified_by", c => { c.Nullable(); })
//					.AddDateTimeColumn("last_modified_on", c => { c.NotNullable().WithAutoDefaultValue(); });

//			})
//			.WithConstraints(constraints =>
//			{
//				constraints
//					.AddPrimaryKeyConstraint("pk_repository_files_id", c => { c.WithColumns("id"); });
//			})
//			.WithIndexes(indexes =>
//			{
//				indexes
//					.AddBTreeIndex("ix_repository_file_id", i => { i.WithColumns("id"); })
//					.AddHashIndex("ix_repository_file_filename", i => { i.WithColumn("filename"); });
//			});

//		#endregion
//	}
//}