using WebVella.Tefter.Database;
using WebVella.Tefter.Migrations;
using WebVella.Tefter.Talk.Services;

namespace WebVella.Tefter.Talk.Migrations;


[TfApplicationMigration(Constants.TALK_APP_ID_STRING, "2024.10.7.1")]
public class TalkMigration2024100701 : ITfApplicationMigration
{
	public async Task MigrateDataAsync(TfApplicationBase app, IServiceProvider serviceprovider, IDatabaseService dbService)
	{
	}

	public async Task MigrateStructureAsync(TfApplicationBase app, DatabaseBuilder dbBuilder)
	{

		dbBuilder
			.WithTableBuilder("talk_thread")
			.WithColumns(columns =>
			{
				columns
					.AddDateTimeColumn("last_updated_on", c => { c.WithoutAutoDefaultValue().Nullable(); })
					.AddDateTimeColumn("deleted_on", c => { c.WithoutAutoDefaultValue().Nullable(); });
			});
	}
}
