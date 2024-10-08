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

				var (id,threads) = talkService.CreateThread(thread);


			}


		}
	}
}
