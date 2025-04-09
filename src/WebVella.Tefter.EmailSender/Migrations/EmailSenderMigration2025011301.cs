namespace WebVella.Tefter.Talk.Migrations;


[TfApplicationMigration(EmailSenderConstants.APP_ID_STRING, "2025.01.13.1")]
public class EmailSenderMigration2025011301 : ITfApplicationMigration
{
	public async Task MigrateDataAsync(ITfApplicationAddon app, IServiceProvider serviceprovider, ITfDatabaseService dbService)
	{
		await Task.Delay(0);
	}

	public async Task MigrateStructureAsync(ITfApplicationAddon app, TfDatabaseBuilder dbBuilder)
	{
		await Task.Delay(0);

		dbBuilder
			.WithTableBuilder("email_message")
			.WithColumns(columns =>
			{
				columns
					.AddTextColumn("related_row_ids", c => { c.NotNullable().WithDefaultValue("[]"); });
			});
	}
}