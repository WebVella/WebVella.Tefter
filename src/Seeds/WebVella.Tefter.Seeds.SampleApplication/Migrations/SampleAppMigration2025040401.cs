//using WebVella.Tefter.Database;
//using WebVella.Tefter.Migrations;

//namespace WebVella.Tefter.Seeds.SampleApplication.Migrations;


//[TfApplicationMigration(SampleAppConstants.APP_ID_STRING, "2025.04.04.01")]
//public class SampleAppMigration2025040401 : ITfApplicationMigration
//{
//	public Task MigrateDataAsync(ITfApplication app, IServiceProvider serviceprovider, ITfDatabaseService dbService)
//	{
//		return Task.CompletedTask;
//	}

//	public Task MigrateStructureAsync(ITfApplication app, TfDatabaseBuilder dbBuilder)
//	{
//		dbBuilder
//			.NewTableBuilder(Guid.NewGuid(), "talk_channel")
//			.WithColumns(columns =>
//			{
//				columns
//					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
//					.AddShortTextColumn("name", c => { c.NotNullable(); })
//					.AddShortTextColumn("shared_key", c => { c.NotNullable(); })
//					.AddShortTextColumn("count_shared_column_name", c => { c.Nullable(); });
//			})
//			.WithConstraints(constraints =>
//			{
//				constraints
//					.AddPrimaryKeyConstraint("pk_talk_channel_id", c => { c.WithColumns("id"); });
//			});

//		dbBuilder
//			.NewTableBuilder(Guid.NewGuid(), "talk_thread")
//			.WithColumns(columns =>
//			{
//				columns
//					.AddGuidColumn("id", c => { c.WithAutoDefaultValue().NotNullable(); })
//					.AddGuidColumn("channel_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
//					.AddGuidColumn("thread_id", c => { c.Nullable(); })
//					.AddShortIntegerColumn("type", c => { c.NotNullable(); })
//					.AddTextColumn("content", c => { c.NotNullable(); })
//					.AddGuidColumn("user_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
//					.AddDateTimeColumn("created_on", c => { c.WithAutoDefaultValue().NotNullable(); });
//			})
//			.WithConstraints(constraints =>
//			{
//				constraints
//					.AddPrimaryKeyConstraint("pk_talk_thread_id", c => { c.WithColumns("id"); })
//					.AddForeignKeyConstraint("fk_talk_thread_channel", c =>
//					{
//						c
//						.WithForeignTable("talk_channel")
//						.WithForeignColumns("id")
//						.WithColumns("channel_id");
//					});
//			});


//		dbBuilder
//			.NewTableBuilder(Guid.NewGuid(), "talk_related_sk")
//			.WithColumns(columns =>
//			{
//				columns
//					.AddGuidColumn("id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
//					.AddGuidColumn("thread_id", c => { c.WithoutAutoDefaultValue().NotNullable(); });
//			})
//			.WithConstraints(constraints =>
//			{
//				constraints
//					.AddPrimaryKeyConstraint("pk_talk_related_sk_id", c => { c.WithColumns("id", "thread_id"); })
//					.AddForeignKeyConstraint("fk_talk_related_sk_thread", c =>
//					{
//						c
//						.WithForeignTable("talk_thread")
//						.WithForeignColumns("id")
//						.WithColumns("thread_id");
//					});
//			});

//		return Task.CompletedTask;
//	}
//}
