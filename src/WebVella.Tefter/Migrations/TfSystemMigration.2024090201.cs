namespace WebVella.Tefter.Migrations;

[TfSystemMigration("2024.9.2.1")]
internal class TefterSystemMigration2024090201 : TfSystemMigration
{
	public override void MigrateStructure(TfDatabaseBuilder dbBuilder)
	{
		#region  TABLE: space_view_column

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "space_view_column")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddGuidColumn("space_view_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
					.AddShortTextColumn("query_name", c => { c.NotNullable(); })
					.AddShortTextColumn("title", c => { c.NotNullable(); })
					.AddShortIntegerColumn("position", c => { c.NotNullable(); })
					.AddTextColumn("full_type_name", c => { c.NotNullable(); })
					.AddTextColumn("full_component_type_name", c => { c.NotNullable(); })
					.AddTextColumn("data_mapping_json", c => { c.NotNullable(); })
					.AddTextColumn("custom_options_json", c => { c.NotNullable(); });


			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_space_view_column_id", c => { c.WithColumns("id"); })
					.AddForeignKeyConstraint("fk_space_view_column_space_view", c =>
					{
						c
						.WithForeignTable("space_view")
						.WithForeignColumns("id")
						.WithColumns("space_view_id");
					});
			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_space_view_column_id", i => { i.WithColumns("id"); })
					.AddBTreeIndex("ix_space_view_column_space_view_id", i => { i.WithColumns("space_view_id"); });
			});

		#endregion

	}
}