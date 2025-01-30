namespace WebVella.Tefter.Tests;

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
			IIdentityManager identityManager = ServiceProvider.GetRequiredService<IIdentityManager>();
			ITfDataManager dataManager = ServiceProvider.GetRequiredService<ITfDataManager>();


			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var user = identityManager.GetUser("rumen@webvella.com").Value;

				TalkChannel channel = new TalkChannel
				{
					Id = Guid.NewGuid(),
					Name = "Test Channel",
					SharedKey = "talk_shared_key",
					CountSharedColumnName = ""
				};

				var channelResult = talkService.CreateChannel(channel);
				channelResult.IsSuccess.Should().BeTrue();

				channel = channelResult.Value;
				channel.Should().NotBeNull();

				channel.Name = "Test channel 1";
				channelResult = talkService.UpdateChannel(channel);
				channelResult.IsSuccess.Should().BeTrue();

				channel.Name.Should().Be("Test channel 1");

				channelResult = talkService.DeleteChannel(channel.Id);
				channelResult.IsSuccess.Should().BeTrue();
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
			IIdentityManager identityManager = ServiceProvider.GetRequiredService<IIdentityManager>();
			ITfDataManager dataManager = ServiceProvider.GetRequiredService<ITfDataManager>();


			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var (provider, spaceData) = await SpaceEnvUtility.CreateTestStructureAndData(ServiceProvider, dbService);
				var dataTable = dataManager.QueryDataProvider(provider).Value;

				List<Guid> rowIds = new List<Guid>();
				for(int i = 0; i < 5 ; i++)
					rowIds.Add((Guid)dataTable.Rows[i]["tf_id"]);

				var user = identityManager.GetUser("rumen@webvella.com").Value;

				TalkChannel channel = new TalkChannel
				{
					Id = Guid.NewGuid(),
					Name = "Test Channel",
					SharedKey = "shared_key_text",
					CountSharedColumnName = ""
				};
				var channelCreated = talkService.CreateChannel(channel);

				Guid skId = dataManager.GetId("shared_key_value", "1");

				CreateTalkThread thread = new CreateTalkThread
				{
					ChannelId = channel.Id,
					Content = "content",
					Type = TalkThreadType.Comment,
					UserId = user.Id,
					RowIds = rowIds.ToList(),
					DataProviderId = provider.Id,
				};

				var id1 = talkService.CreateThread(thread).Value;
				
				var th = talkService.GetThread(id1).Value;
				th.Should().NotBeNull();

				var threads = talkService.GetThreads(channel.Id, null).Value;
				threads.Count.Should().Be(1);

				var relSKId = threads[0].RelatedSK.Keys.First();

				threads = talkService.GetThreads(channel.Id, relSKId).Value;
				threads.Count.Should().Be(1);

				CreateTalkSubThread thread2 = new CreateTalkSubThread
				{
					ThreadId = id1,
					Content = "sub thread content2",
					UserId = user.Id,
				};

				var id2 = talkService.CreateSubThread(thread2).Value;
				threads = talkService.GetThreads(channel.Id, relSKId).Value;
				threads.Count.Should().Be(1);
				threads[0].SubThread.Count.Should().Be(1);

				CreateTalkSubThread thread3 = new CreateTalkSubThread
				{
					ThreadId = id1,
					Content = "sub thread content3",
					UserId = user.Id,
					VisibleInChannel = true
				};

				var id3 = talkService.CreateSubThread(thread3).Value;
				threads = talkService.GetThreads(channel.Id, relSKId).Value;
				threads.Count.Should().Be(2);
			}
		}
	}
}
