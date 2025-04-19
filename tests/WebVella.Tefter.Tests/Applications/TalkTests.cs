namespace WebVella.Tefter.Tests.Applications;

using WebVella.Tefter.Talk.Models;
using WebVella.Tefter.Talk.Services;

public partial class TalkTests : BaseTest
{
	[Fact]
	public async Task Talk_Channel_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITalkService talkService = ServiceProvider.GetRequiredService<ITalkService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();


			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var user = tfService.GetUser("rumen@webvella.com");

				TalkChannel channel = new TalkChannel
				{
					Id = Guid.NewGuid(),
					Name = "Test Channel",
					JoinKey = "talk_shared_key",
					CountSharedColumnName = ""
				};

				channel = talkService.CreateChannel(channel);
				channel.Should().NotBeNull();

				channel.Name = "Test channel 1";
				channel = talkService.UpdateChannel(channel);
				channel.Name.Should().Be("Test channel 1");
				talkService.DeleteChannel(channel.Id);
			}
		}
	}

	[Fact]
	public async Task Talk_Thread_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITalkService talkService = ServiceProvider.GetRequiredService<ITalkService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var (provider, spaceData) = await CreateTestStructureAndData(ServiceProvider, dbService);
				var dataTable = tfService.QueryDataProvider(provider);

				List<Guid> rowIds = new List<Guid>();
				for (int i = 0; i < 5; i++)
					rowIds.Add((Guid)dataTable.Rows[i]["tf_id"]);

				var user = tfService.GetUser("admin@tefter.bg");

				TalkChannel channel = new TalkChannel
				{
					Id = Guid.NewGuid(),
					Name = "Test Channel",
					JoinKey = "shared_key_text",
					CountSharedColumnName = ""
				};
				var channelCreated = talkService.CreateChannel(channel);

				Guid skId = tfService.GetId("shared_key_value", "1");

				CreateTalkThread thread = new CreateTalkThread
				{
					ChannelId = channel.Id,
					Content = "content",
					Type = TalkThreadType.Comment,
					UserId = user.Id,
					RowIds = rowIds.ToList(),
					DataProviderId = provider.Id,
				};

				var id1 = talkService.CreateThread(thread);

				var th = talkService.GetThread(id1);
				th.Should().NotBeNull();

				var threads = talkService.GetThreads(channel.Id, null);
				threads.Count.Should().Be(1);

				var relSKId = threads[0].RelatedSK.Keys.First();

				threads = talkService.GetThreads(channel.Id, relSKId);
				threads.Count.Should().Be(1);

				CreateTalkSubThread thread2 = new CreateTalkSubThread
				{
					ThreadId = id1,
					Content = "sub thread content2",
					UserId = user.Id,
				};

				var id2 = talkService.CreateSubThread(thread2);
				threads = talkService.GetThreads(channel.Id, relSKId);
				threads.Count.Should().Be(1);
				threads[0].SubThread.Count.Should().Be(1);

				CreateTalkSubThread thread3 = new CreateTalkSubThread
				{
					ThreadId = id1,
					Content = "sub thread content3",
					UserId = user.Id,
					VisibleInChannel = true
				};

				var id3 = talkService.CreateSubThread(thread3);
				threads = talkService.GetThreads(channel.Id, relSKId);
				threads.Count.Should().Be(2);
			}
		}
	}
}
