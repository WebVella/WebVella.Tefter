namespace WebVella.Tefter.Migrations;

[TefterSystemMigration("2024.7.10.1")]
internal class TefterSystemMigration2024071001 : TefterSystemMigration
{
	public override void MigrateStructure(DatabaseBuilder dbBuilder)
	{
		#region  TABLE: data_provider_synchronize_task

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "data_provider_synchronize_task")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddGuidColumn("data_provider_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
					.AddTextColumn("policy_json", c => { c.NotNullable().WithDefaultValue("{}"); })
					.AddShortIntegerColumn("status", c => { c.NotNullable(); })
					.AddDateTimeColumn("created_on", c => { c.NotNullable().WithAutoDefaultValue(); })
					.AddDateTimeColumn("started_on", c => { c.Nullable().WithoutAutoDefaultValue(); })
					.AddDateTimeColumn("completed_on", c => { c.Nullable().WithoutAutoDefaultValue(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_dp_synchronize_task_id", c => { c.WithColumns("id"); });
			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_dp_task_id", i => { i.WithColumns("id"); })
					.AddBTreeIndex("ix_dp_synchronize_task_provider_id", i => { i.WithColumns("data_provider_id"); })
					.AddBTreeIndex("ix_dp_synchronize_task_created_on", i => { i.WithColumns("created_on"); });
			});

		#endregion

		#region  TABLE: data_provider_synchronize_result_info

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "data_provider_synchronize_result_info")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddGuidColumn("task_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
					.AddDateTimeColumn("created_on", c => { c.NotNullable().WithAutoDefaultValue(); })
					.AddIntegerColumn("tf_row_index", c => { c.Nullable(); })
					.AddGuidColumn("tf_id", c => { c.Nullable(); })
					.AddShortTextColumn("info", c => { c.Nullable(); })
					.AddShortTextColumn("warning", c => { c.Nullable(); })
					.AddShortTextColumn("error", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_dp_synchronize_result_info_id", c => { c.WithColumns("id"); })
					.AddForeignKeyConstraint("fk_dp_task_result_info", c =>
					{
						c
						.WithForeignTable("data_provider_synchronize_task")
						.WithForeignColumns("id")
						.WithColumns("task_id");
					});
			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_dp_synchronize_result_info_id", i => { i.WithColumns("id"); })
					.AddBTreeIndex("ix_dp_synchronize_result_info_task_id", i => { i.WithColumns("task_id"); })
					.AddBTreeIndex("ix_dp_synchronize_result_info_created_on", i => { i.WithColumns("created_on"); });
			});

		#endregion
	}
}
