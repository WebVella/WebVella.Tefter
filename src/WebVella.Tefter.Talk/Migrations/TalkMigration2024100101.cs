using WebVella.Tefter.Database;
using WebVella.Tefter.Migrations;
using WebVella.Tefter.Talk.Services;

namespace WebVella.Tefter.Talk.Migrations;


[TfApplicationMigration(TalkConstants.TALK_APP_ID_STRING, "2024.10.1.1")]
public class TalkMigration2024100101 : ITfApplicationMigration
{
	public async Task MigrateDataAsync(ITfApplication app, IServiceProvider serviceprovider, ITfDatabaseService dbService)
	{
		await Task.Delay(0);
	}

	public async Task MigrateStructureAsync(ITfApplication app, TfDatabaseBuilder dbBuilder)
	{
		await Task.Delay(0);

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "talk_channel")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
					.AddShortTextColumn("name", c => { c.NotNullable(); })
					.AddShortTextColumn("shared_key", c => { c.NotNullable(); })
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
					.AddGuidColumn("channel_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
					.AddGuidColumn("thread_id", c => { c.Nullable(); })
					.AddShortIntegerColumn("type", c => { c.NotNullable(); })
					.AddTextColumn("content", c => { c.NotNullable(); })
					.AddGuidColumn("user_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
					.AddDateTimeColumn("created_on", c => { c.WithAutoDefaultValue().NotNullable(); });
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
		

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "talk_related_sk")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
					.AddGuidColumn("thread_id", c => { c.WithoutAutoDefaultValue().NotNullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_talk_related_sk_id", c => { c.WithColumns("id", "thread_id"); })
					.AddForeignKeyConstraint("fk_talk_related_sk_thread", c =>
					{
						c
						.WithForeignTable("talk_thread")
						.WithForeignColumns("id")
						.WithColumns("thread_id");
					});
			});

		
	}
}
