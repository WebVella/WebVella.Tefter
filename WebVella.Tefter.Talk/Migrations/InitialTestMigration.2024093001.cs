using WebVella.Tefter.Database;
using WebVella.Tefter.Migrations;
using WebVella.Tefter.Talk.Services;

namespace WebVella.Tefter.Talk.Migrations;


[TfApplicationMigration(Constants.TALK_APP_ID_STRING, "2024.7.10.1")]
public class InitialTestMigration : ITfApplicationMigration
{
	public async Task MigrateDataAsync(TfApplicationBase app, IServiceProvider serviceprovider, IDatabaseService dbService)
	{
		var talkService = serviceprovider.GetRequiredService<ITalkService>();
		talkService.InsertSampleData();
	}

	public async Task MigrateStructureAsync(TfApplicationBase app, DatabaseBuilder dbBuilder)
	{
		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "talk_sample_table")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_talk_sample_table_id", c => { c.WithColumns("id"); });
			});
	}
}
