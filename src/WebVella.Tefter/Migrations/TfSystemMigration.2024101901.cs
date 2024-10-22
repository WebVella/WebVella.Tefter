namespace WebVella.Tefter.Migrations;

[TfSystemMigration("2024.10.19.1")]
internal class TefterSystemMigration2024101901 : TfSystemMigration
{
	public override void MigrateStructure(DatabaseBuilder dbBuilder)
	{
		#region  TABLE: space_node

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "space_node")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddGuidColumn("parent_id", c => { c.Nullable().WithoutAutoDefaultValue(); })
					.AddGuidColumn("space_id", c => { c.NotNullable().WithoutAutoDefaultValue(); })
					.AddShortTextColumn("name", c => { c.NotNullable(); })
					.AddShortTextColumn("icon", c => { c.NotNullable(); })
					.AddShortIntegerColumn("position", c => { c.NotNullable(); })
					.AddShortIntegerColumn("type", c => { c.NotNullable(); })
					.AddTextColumn("component_type", c => { c.NotNullable().WithDefaultValue(""); })
					.AddTextColumn("component_settings_json", c => { c.NotNullable().WithDefaultValue("{}"); });

			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_space_node_id", c => { c.WithColumns("id"); })
					.AddForeignKeyConstraint("fk_space_node_space", c =>
					{
						c.WithForeignTable("space")
						.WithForeignColumns("id")
						.WithColumns("space_id");
					})
					.AddForeignKeyConstraint("fk_space_node_parent", c =>
					{
						c.WithForeignTable("space_node")
						.WithForeignColumns("id")
						.WithColumns("parent_id");
					});
			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_space_node_id", i => { i.WithColumns("id"); })
					.AddBTreeIndex("ix_space_node_space_id", i => { i.WithColumns("space_id"); });
			});

		#endregion
	}
}