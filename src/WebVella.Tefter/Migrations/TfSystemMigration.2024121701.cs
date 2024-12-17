namespace WebVella.Tefter.Migrations;

[TfSystemMigration("2024.12.17.1")]
internal class TefterSystemMigration2024121701 : TfSystemMigration
{
	public override void MigrateStructure(TfDatabaseBuilder dbBuilder)
	{
		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "template")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddShortTextColumn("name", c => { c.NotNullable(); })
					.AddShortTextColumn("icon", c => { c.Nullable(); })
					.AddTextColumn("description", c => { c.NotNullable().WithDefaultValue(""); })
					.AddTextColumn("used_columns_json", c => { c.NotNullable().WithDefaultValue("[]"); })
					.AddBooleanColumn("is_enabled", c => { c.NotNullable().WithDefaultValue(true); })
					.AddBooleanColumn("is_selectable", c => { c.NotNullable().WithDefaultValue(true); })
					.AddShortIntegerColumn("result_type", c => { c.NotNullable().WithDefaultValue(0); })
					.AddTextColumn("settings_json", c => { c.NotNullable().WithDefaultValue("{}"); })
					.AddTextColumn("content_processor_type", c => { c.NotNullable(); })
					.AddDateTimeColumn("created_on", c => { c.NotNullable().WithAutoDefaultValue(); })
					.AddDateTimeColumn("modified_on", c => { c.NotNullable().WithAutoDefaultValue(); })
					.AddGuidColumn("created_by", c => { c.Nullable(); })
					.AddGuidColumn("modified_by", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_template_id", c => { c.WithColumns("id"); });
			});
	}
}