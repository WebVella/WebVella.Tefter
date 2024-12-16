using WebVella.Tefter.Database;
using WebVella.Tefter.Migrations;
using WebVella.Tefter.Talk.Services;

namespace WebVella.Tefter.Talk.Migrations;


[TfApplicationMigration(TalkConstants.TALK_APP_ID_STRING, "2024.12.5.1")]
public class TalkMigration2024120501 : ITfApplicationMigration
{
	public async Task MigrateDataAsync(ITfApplication app, IServiceProvider serviceprovider, ITfDatabaseService dbService)
	{
		await Task.Delay(0);
	}

	public async Task MigrateStructureAsync(ITfApplication app, TfDatabaseBuilder dbBuilder)
	{
		await Task.Delay(0);

		dbBuilder
			.WithTableBuilder("talk_channel")
			.WithColumns(columns =>
			{
				columns.WithShortTextColumn("shared_key", c=>c.Nullable() );
			});
	}
}
