using Microsoft.Extensions.DependencyInjection;
using WebVella.Tefter.Database;
using WebVella.Tefter.Migrations;
using WebVella.Tefter.Services;
using WebVella.Tefter.Talk.Services;

namespace WebVella.Tefter.Talk.Migrations;


[TfApplicationMigration(TalkApp.ID, "2024.10.1.1")]
public class TalkMigration2024100101 : ITfApplicationMigration
{
	public async Task MigrateDataAsync(ITfApplicationAddon app, IServiceProvider serviceProvider, ITfDatabaseService dbService)
	{
		await Task.Delay(0);
		var counterColumn = $"{TfConstants.TF_SHARED_COLUMN_PREFIX}{TfConstants.TEFTER_DEFAULT_OBJECT_NAME}_talk_counter";
        //CREATE DEFAULT LONG SHARED COLUMN
        ITfService tfService = serviceProvider.GetService<ITfService>()!;
        _ = tfService.CreateSharedColumn(
                new TfSharedColumn
                {
                    Id = new TalkApp().AddonId,
                    DataIdentity = TfConstants.TEFTER_DEFAULT_OBJECT_NAME,
                    DbName = counterColumn,
                    DbType = TfDatabaseColumnType.LongInteger,
                    IncludeInTableSearch = false
                }
            );

        ITalkService assetService = serviceProvider.GetService<ITalkService>()!;
        _ = assetService.CreateChannel(new TalkChannel
        {
            Id = new TalkApp().AddonId,
            Name = TfConstants.TEFTER_DEFAULT_OBJECT_NAME,
            DataIdentity = TfConstants.TEFTER_DEFAULT_OBJECT_NAME,
            CountSharedColumnName = counterColumn
        });


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
					.AddShortTextColumn("identity_row_id", c => { c.AsSha1ExpressionFromColumns("id"); })
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
		
		#region  TABLE: Thread_TAG

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "talk_thread_tag")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("talk_thread_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
					.AddGuidColumn("tag_id", c => { c.WithoutAutoDefaultValue().NotNullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_talk_thread_tags", c => { c.WithColumns("talk_thread_id", "tag_id"); })
					.AddForeignKeyConstraint("fk_talk_thread_tags_talk_thread", c =>
					{
						c.WithForeignTable("talk_thread")
							.WithForeignColumns("id")
							.WithColumns("talk_thread_id");
					})
					.AddForeignKeyConstraint("fk_talk_thread_tags_tag", c =>
					{
						c.WithForeignTable("tf_tag")
							.WithForeignColumns("id")
							.WithColumns("tag_id");
					});
			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_talk_thread_tags_talk_thread_id", i => { i.WithColumns("talk_thread_id"); })
					.AddBTreeIndex("ix_talk_thread_tags_tag_id", i => { i.WithColumns("tag_id"); });
			});

		#endregion
		
		#region  TABLE: Thread_MENTION

		dbBuilder
			.NewTableBuilder(Guid.NewGuid(), "talk_thread_mention")
			.WithColumns(columns =>
			{
				columns
					.AddGuidColumn("talk_thread_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
					.AddGuidColumn("user_id", c => { c.WithoutAutoDefaultValue().NotNullable(); });
			})
			.WithConstraints(constraints =>
			{
				constraints
					.AddPrimaryKeyConstraint("pk_talk_thread_mention", c => { c.WithColumns("talk_thread_id", "user_id"); })
					.AddForeignKeyConstraint("fk_talk_thread_mention_talk_thread", c =>
					{
						c.WithForeignTable("talk_thread")
							.WithForeignColumns("id")
							.WithColumns("talk_thread_id");
					})
					.AddForeignKeyConstraint("fk_talk_thread_mention_user", c =>
					{
						c.WithForeignTable("tf_user")
							.WithForeignColumns("id")
							.WithColumns("user_id");
					});
			})
			.WithIndexes(indexes =>
			{
				indexes
					.AddBTreeIndex("ix_talk_thread_mention_talk_thread_id", i => { i.WithColumns("talk_thread_id"); })
					.AddBTreeIndex("ix_talk_thread_mention_user_id", i => { i.WithColumns("user_id"); });
			});

		#endregion		
		
	}
}
