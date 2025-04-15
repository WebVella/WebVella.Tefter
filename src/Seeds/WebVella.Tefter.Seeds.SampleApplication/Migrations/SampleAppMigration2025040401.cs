using Microsoft.Extensions.DependencyInjection;
namespace WebVella.Tefter.Seeds.SampleApplication.Migrations;

[TfApplicationMigration(SampleApp.ID, "2025.04.04.01")]
public class SampleAppMigration2025040401 : ITfApplicationMigration
{
	public Task MigrateDataAsync(ITfApplicationAddon app, IServiceProvider serviceProvider, ITfDatabaseService dbService)
	{
		ISampleAppService sampleAppService = serviceProvider.GetService<ISampleAppService>();

		sampleAppService.AddNote("Test note 1");
		sampleAppService.AddNote("Test note 2");
		sampleAppService.AddNote("Test note 3");

		return Task.CompletedTask;
	}

	public Task MigrateStructureAsync(ITfApplicationAddon app, TfDatabaseBuilder dbBuilder)
	{
		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "sample_app_notes")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddShortTextColumn("note_text", c => { c.NotNullable().WithDefaultValue(""); })
					.AddDateTimeColumn("created_on", c => { c.WithDefaultValue(DateTime.UtcNow); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_sample_app_note_id", c => { c.WithColumns("id"); });
			});


		return Task.CompletedTask;
	}
}
