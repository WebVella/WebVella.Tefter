using WebVella.Tefter.Database;
using WebVella.Tefter.Migrations;
using WebVella.Tefter.Talk.Services;

namespace WebVella.Tefter.Talk.Migrations;


[TfApplicationMigration(Constants.TALK_APP_ID_STRING, "2024.10.8.1")]
public class TalkMigration2024100801 : ITfApplicationMigration
{
	public async Task MigrateDataAsync(ITfApplication app, IServiceProvider serviceprovider, IDatabaseService dbService)
	{
		await Task.Delay(0);
	}

	public async Task MigrateStructureAsync(ITfApplication app, DatabaseBuilder dbBuilder)
	{
		await Task.Delay(0);

		dbBuilder
			.WithTableBuilder("talk_thread")
			.WithColumns(columns =>
			{
				columns
					.AddBooleanColumn("visible_in_channel", c => { c.WithDefaultValue(false); });
			});
	}
}
