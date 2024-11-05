namespace WebVella.Tefter.Migrations;

[TfSystemMigration("2024.9.20.1")]
internal class TefterSystemMigration2024092001 : TfSystemMigration
{
	public override void MigrateStructure(TfDatabaseBuilder dbBuilder)
	{

		#region  TABLE: bookmark

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "bookmark")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddShortTextColumn("name", c => { c.NotNullable(); })
					.AddTextColumn("description", c => { c.NotNullable(); })
					.AddTextColumn("url", c => { c.Nullable(); })
					.AddDateTimeColumn("created_on", c => { c.NotNullable().WithAutoDefaultValue(); })
					.AddGuidColumn("user_id", c => { c.NotNullable(); })
					.AddGuidColumn("space_view_id", c => { c.NotNullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_bookmark_id", c => { c.WithColumns("id"); })
					.AddForeignKeyConstraint("fk_bookmark_space_view", c =>
					{
						c.WithForeignTable("space_view")
						.WithForeignColumns("id")
						.WithColumns("space_view_id");
					});
			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_bookmark_id", i => { i.WithColumns("id"); })
					.AddBTreeIndex("ix_bookmark_user_id", i => { i.WithColumns("user_id"); });
			});

		#endregion

		#region  TABLE: tag

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "tag")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddShortTextColumn("label", c => { c.NotNullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_tag_id", c => { c.WithColumns("id"); });
			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_tag_id", i => { i.WithColumns("id"); })
					.AddBTreeIndex("ix_tag_label", i => { i.WithColumns("label"); });
			});

		#endregion

		#region  TABLE: bookmark_tags

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "bookmark_tags")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("bookmark_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
					.AddGuidColumn("tag_id", c => { c.WithoutAutoDefaultValue().NotNullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_bookmark_tags", c => { c.WithColumns("bookmark_id", "tag_id"); })
					.AddForeignKeyConstraint("fk_bookmark_tags_bookmark", c =>
					{
						c.WithForeignTable("bookmark")
						.WithForeignColumns("id")
						.WithColumns("bookmark_id");
					})
					.AddForeignKeyConstraint("fk_bookmark_tags_tag", c =>
					{
						c.WithForeignTable("tag")
						.WithForeignColumns("id")
						.WithColumns("tag_id");
					});
			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_bookmark_tags_bookmark_id", i => { i.WithColumns("bookmark_id"); })
					.AddBTreeIndex("ix_bookmark_tags_tag_id", i => { i.WithColumns("tag_id"); });
			});

		#endregion

	}
}