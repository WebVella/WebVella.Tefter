namespace WebVella.Tefter.Tests;

using System.Drawing;
using WebVella.Tefter.Talk.Models;
using WebVella.Tefter.Talk.Services;


public partial class TalkTests : BaseTest
{
	[Fact]
	public async Task Talk_Channel()
	{
		using (await locker.LockAsync())
		{
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();
			ITalkService talkService = ServiceProvider.GetRequiredService<ITalkService>();
			IIdentityManager identityManager = ServiceProvider.GetRequiredService<IIdentityManager>();
			IDataManager dataManager = ServiceProvider.GetRequiredService<IDataManager>();


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
				var channelCreated =  talkService.CreateChannel(channel);

				Guid skId = dataManager.GetId("shared_key_value", "1").Value;

				CreateTalkThread thread = new CreateTalkThread
				{
					ChannelId = channel.Id,
					ThreadId = null,
					Content = "content",
					Type = TalkThreadType.Comment,
					UserId = user.Id,
					RelatedSK = new List<Guid> { skId }
				};

				var (id1,threads) = talkService.CreateThread(thread).Value;

				CreateTalkThread thread2 = new CreateTalkThread
				{
					ChannelId = channel.Id,
					ThreadId = id1,
					Content = "sub thread content2",
					Type = TalkThreadType.Comment,
					UserId = user.Id,
					RelatedSK = new List<Guid> { skId }
				};

				var (id2, threads2) = talkService.CreateThread(thread2).Value;

				CreateTalkThread thread3 = new CreateTalkThread
				{
					ChannelId = channel.Id,
					ThreadId = id1,
					Content = "sub thread content3",
					Type = TalkThreadType.Comment,
					UserId = user.Id,
					RelatedSK = new List<Guid> { skId },
					VisibleInChannel = true
				};

				var (id3, threads3) = talkService.CreateThread(thread3).Value;

				var threadList = talkService.GetThreads(channel.Id, skId).Value;

				CreateTalkThread thread4 = new CreateTalkThread
				{
					ChannelId = channel.Id,
					ThreadId = id1,
					Content = "sub thread content3",
					Type = TalkThreadType.Comment,
					UserId = user.Id,
					VisibleInChannel = true
				};

				var (id4, threads4) = talkService.CreateThread(thread4).Value;
			}


		}
	}
}
