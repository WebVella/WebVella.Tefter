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


			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				TalkChannel channel = new TalkChannel
				{
					Id = Guid.NewGuid(),
					Name = "Test Channel",
					SharedKey = "talk_shared_key",
					CountSharedColumnName = ""
				};
				var channelCreated =  talkService.CreateChannel(channel);

				TalkThread thread = new CreateTalkThread
				{
					ChannelId = channel.Id,
					ThreadId = null,
					Content = "content",

				}
			}


		}
	}
}
