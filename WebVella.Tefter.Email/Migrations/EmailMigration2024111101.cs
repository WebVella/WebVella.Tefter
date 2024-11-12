namespace WebVella.Tefter.Talk.Migrations;


[TfApplicationMigration(TfEmailConstants.APP_ID_STRING, "2024.11.11.1")]
public class EmailMigration2024111101 : ITfApplicationMigration
{
	public async Task MigrateDataAsync(ITfApplication app, IServiceProvider serviceprovider, ITfDatabaseService dbService)
	{
		await Task.Delay(0);
	}

	public async Task MigrateStructureAsync(ITfApplication app, TfDatabaseBuilder dbBuilder)
	{
		await Task.Delay(0);

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "email_message")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddShortTextColumn("subject", c => { c.NotNullable().WithDefaultValue(string.Empty); })
					.AddTextColumn("content_text", c => { c.NotNullable().WithDefaultValue(string.Empty); })
					.AddTextColumn("content_html", c => { c.NotNullable().WithDefaultValue(string.Empty); })
					.AddDateTimeColumn("sent_on", c => { c.Nullable(); })
					.AddDateTimeColumn("created_on", c => { c.NotNullable().WithAutoDefaultValue(); })
					.AddTextColumn("server_error", c => { c.Nullable(); })
					.AddShortIntegerColumn("retries_count", c => { c.NotNullable().WithDefaultValue(0); })
					.AddShortIntegerColumn("priority", c => { c.NotNullable().WithDefaultValue(1); })
					.AddTextColumn("reply_to_email", c => { c.NotNullable().WithDefaultValue(string.Empty); })
					.AddDateTimeColumn("scheduled_on", c => { c.NotNullable().WithAutoDefaultValue(); })
					.AddShortIntegerColumn("status", c => { c.NotNullable().WithDefaultValue(0); })
					.AddTextColumn("sender", c => { c.NotNullable().WithDefaultValue(string.Empty); })
					.AddTextColumn("recipients", c => { c.NotNullable().WithDefaultValue(string.Empty); })
					.AddTextColumn("recipients_cc", c => { c.NotNullable().WithDefaultValue(string.Empty); })
					.AddTextColumn("recipients_bcc", c => { c.NotNullable().WithDefaultValue(string.Empty); })
					.AddTextColumn("attachments", c => { c.NotNullable().WithDefaultValue(string.Empty); })
					.AddTextColumn("x_search", c => { c.NotNullable().WithDefaultValue(string.Empty); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_email_message_id", c => { c.WithColumns("id"); });
			})
			.WithIndexes(indexes => {
				indexes
					.AddHashIndex("idx_email_message_x_search", c => { c.WithColumn("x_search"); })
					.AddBTreeIndex("idx_email_message_id", c => { c.WithColumns("id"); })
					.AddBTreeIndex("idx_email_message_created_on", c => { c.WithColumns("created_on"); })
					.AddBTreeIndex("idx_email_message_scheduled_on", c => { c.WithColumns("scheduled_on"); })
					.AddBTreeIndex("idx_email_message_status", c => { c.WithColumns("status"); });
			});
	}
}