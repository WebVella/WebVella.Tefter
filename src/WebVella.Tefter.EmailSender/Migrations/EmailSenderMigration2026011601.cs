namespace WebVella.Tefter.Talk.Migrations;


[TfApplicationMigration(EmailSenderApp.ID, "2026.01.16.1")]
public class EmailSenderMigration2026011601 : ITfApplicationMigration
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
					.AddTextColumn("related_dataset_ids", c => { c.NotNullable().WithDefaultValue("[]"); });
				columns
					.AddTextColumn("related_space_ids", c => { c.NotNullable().WithDefaultValue("[]"); });
			});
	}
}