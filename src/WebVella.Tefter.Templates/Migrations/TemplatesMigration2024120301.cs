namespace WebVella.Tefter.Assets.Migrations;


[TfApplicationMigration(TemplatesConstants.TEMPLATES_APP_ID_STRING, "2024.12.3.1")]
public class TemplatesMigration2024120301 : ITfApplicationMigration
{
	public async Task MigrateDataAsync(ITfApplication app, IServiceProvider serviceprovider, ITfDatabaseService dbService)
	{
		await Task.Delay(0);
	}

	public async Task MigrateStructureAsync(ITfApplication app, TfDatabaseBuilder dbBuilder)
	{
		await Task.Delay(0);

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "templates_template")
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
					.AddPrimaryKeyConstraint("pk_templates_template_id", c => { c.WithColumns("id"); });
			});
	}
}
