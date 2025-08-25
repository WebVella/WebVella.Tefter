using WebVella.Tefter.Database;
using WebVella.Tefter.Migrations;
using WebVella.Tefter.Talk.Services;

namespace WebVella.Tefter.Talk.Migrations;


[TfApplicationMigration(TalkApp.ID, "2024.10.1.1")]
public class TalkMigration2024100101 : ITfApplicationMigration
{
	public async Task MigrateDataAsync(ITfApplicationAddon app, IServiceProvider serviceprovider, ITfDatabaseService dbService)
	{
		await Task.Delay(0);
	}

	public async Task MigrateStructureAsync(ITfApplicationAddon app, TfDatabaseBuilder dbBuilder)
	{
		await Task.Delay(0);

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "talk_channel")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddShortTextColumn("name", c => { c.NotNullable(); })
					.AddShortTextColumn("data_identity", c => { c.Nullable(); })
					.AddShortTextColumn("count_shared_column_name", c => { c.Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_talk_channel_id", c => { c.WithColumns("id"); });
			});

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "talk_thread")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddShortTextColumn("identity_row_id", c => { c.AsGeneratedSHA1FromColumns("id"); })
					.AddGuidColumn("channel_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
					.AddGuidColumn("thread_id", c => { c.Nullable(); })
					.AddShortIntegerColumn("type", c => { c.NotNullable(); })
					.AddTextColumn("content", c => { c.NotNullable(); })
					.AddGuidColumn("user_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
					.AddBooleanColumn("visible_in_channel", c => { c.WithDefaultValue(false); })
					.AddDateTimeColumn("created_on", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddDateTimeColumn("last_updated_on", c => { c.WithoutAutoDefaultValue().Nullable(); })
					.AddDateTimeColumn("deleted_on", c => { c.WithoutAutoDefaultValue().Nullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_talk_thread_id", c => { c.WithColumns("id"); })
					.AddForeignKeyConstraint("fk_talk_thread_channel", c =>
					{
						c
						.WithForeignTable("talk_channel")
						.WithForeignColumns("id")
						.WithColumns("channel_id");
					});
			});		
	}
}
