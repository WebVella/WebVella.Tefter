namespace WebVella.Tefter.Tests.Applications;

using DocumentFormat.OpenXml.Office2010.Excel;
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
					DataIdentity = TfConstants.TF_ROW_ID_DATA_IDENTITY,
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

			//Guid channelId = new Guid("d1749bb8-d41f-4934-a329-09afb981bce8");

			//var thhh = talkService.GetThread(new Guid("447cc5f7-d87f-45ea-859e-f8b8dcb89d5e"));

			//var threads123 = talkService.GetThreads(channelId, dataIdentityValue: "8994cca3dbbdf1fc65a99aa60a8efee258677fe6");

			//return;
			

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var (provider, spaceData) = await CreateTestStructureAndData(ServiceProvider, dbService);
				var dataTable = tfService.QueryDataProvider(provider);

				List<string> rowIdentityIds = new List<string>();
				for (int i = 0; i < 5; i++)
					rowIdentityIds.Add((string)dataTable.Rows[i]["tf_row_id"]);

				var user = tfService.GetDefaultSystemUser();
				if (user == null) throw new Exception("No default system user found");

				TalkChannel channel = new TalkChannel
				{
					Id = Guid.NewGuid(),
					Name = "Test Channel",
					DataIdentity = TfConstants.TF_ROW_ID_DATA_IDENTITY,
					CountSharedColumnName = ""
				};
				var channelCreated = talkService.CreateChannel(channel);

				CreateTalkThreadWithDataIdentityModel thread = new CreateTalkThreadWithDataIdentityModel
				{
					ChannelId = channel.Id,
					Content = "content",
					Type = TalkThreadType.Comment,
					UserId = user.Id,
					DataIdentityValues = rowIdentityIds,
				};

				var id1 = talkService.CreateThread(thread);

				var th = talkService.GetThread(id1.Id);
				th.Should().NotBeNull();

				var threads = talkService.GetThreads(channel.Id, dataIdentityValue: null);
				threads.Count.Should().Be(1);

				var firstDataIdentityValue = rowIdentityIds.First();

				threads = talkService.GetThreads(channel.Id, dataIdentityValue: firstDataIdentityValue);
				threads.Count.Should().Be(1);

				CreateTalkSubThread thread2 = new CreateTalkSubThread
				{
					ThreadId = id1.Id,
					Content = "sub thread content2",
					UserId = user.Id,
				};

				var id2 = talkService.CreateSubThread(thread2);
				threads = talkService.GetThreads(channel.Id, firstDataIdentityValue);
				threads.Count.Should().Be(1);
				threads[0].SubThread.Count.Should().Be(1);

				CreateTalkSubThread thread3 = new CreateTalkSubThread
				{
					ThreadId = id1.Id,
					Content = "sub thread content3",
					UserId = user.Id,
					VisibleInChannel = true
				};

				var id3 = talkService.CreateSubThread(thread3);
				threads = talkService.GetThreads(channel.Id, firstDataIdentityValue);
				threads.Count.Should().Be(2);
			}
		}
	}
}
