﻿namespace WebVella.Tefter.Talk.Migrations;


[TfApplicationMigration(EmailSenderApp.ID, "2024.11.13.1")]
public class EmailSenderMigration2024111301 : ITfApplicationMigration
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
					.AddGuidColumn("user_id", c => { c.Nullable(); });
			});
	}
}