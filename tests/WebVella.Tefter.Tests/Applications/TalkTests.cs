namespace WebVella.Tefter.Tests.Applications;

using AngleSharp.Dom;
using DocumentFormat.OpenXml.Office2010.Excel;
using WebVella.Tefter.Assets.Services;
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
				var (provider, spaceData) = await CreateTestStructureAndData(ServiceProvider, dbService);
				var dataTable = tfService.QueryDataProvider(provider);

				var user = tfService.GetDefaultSystemUser();

				TalkChannel channel = new TalkChannel
				{
					Id = Guid.NewGuid(),
					Name = "Test Channel",
					DataIdentity = TfConstants.TF_ROW_ID_DATA_IDENTITY,
					CountSharedColumnName = "sc_int_row_id"
				};

				channel = talkService.CreateChannel(channel);
				channel.Should().NotBeNull();

				channel = talkService.UpdateChannel( new UpdateTalkChannelModel(channel.Id, "Test channel 1"));
				channel.Name.Should().Be("Test channel 1");


				//try to create channel with same name 
				//should fail with validation exception
				Exception exception = null;
				try { talkService.CreateChannel( channel with { Id = Guid.NewGuid() }); } catch (Exception ex) { exception = ex; }
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));

		

				talkService.DeleteChannel(channel.Id);


			}
		}
	}

	[Fact]
	public async Task Talk_Thread_ByIdentityValues()
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
					CountSharedColumnName = "sc_int_row_id"
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

				var createdThread1 = talkService.CreateThread(thread);

				var th = talkService.GetThread(createdThread1.Id);
				th.Should().NotBeNull();
				th.ConnectedDataIdentityValuesCount.Should().Be(rowIdentityIds.Count);

				var connectedIdentityValues = talkService.GetThreadRelatedIdentityValues(th);
				connectedIdentityValues.Count.Should().Be(rowIdentityIds.Count);

				var threads = talkService.GetThreads(channel.Id, dataIdentityValue: null);
				threads.Count.Should().Be(1);

				var firstDataIdentityValue = rowIdentityIds.First();

				threads = talkService.GetThreads(channel.Id, dataIdentityValue: firstDataIdentityValue);
				threads.Count.Should().Be(1);

				CreateTalkSubThread thread2 = new CreateTalkSubThread
				{
					ThreadId = createdThread1.Id,
					Content = "sub thread content2",
					UserId = user.Id,
				};

				var createdThread2 = talkService.CreateSubThread(thread2);
				threads = talkService.GetThreads(channel.Id, firstDataIdentityValue);
				threads.Count.Should().Be(1);
				threads[0].SubThread.Count.Should().Be(1);

				CreateTalkSubThread thread3 = new CreateTalkSubThread
				{
					ThreadId = createdThread1.Id,
					Content = "sub thread content3",
					UserId = user.Id,
					VisibleInChannel = true
				};

				var createdThread3 = talkService.CreateSubThread(thread3);
				threads = talkService.GetThreads(channel.Id, firstDataIdentityValue);
				threads.Count.Should().Be(2);
			}
		}
	}

	[Fact]
	public async Task Talk_Thread_ByRowIds()
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

				var user = tfService.GetDefaultSystemUser();
				if (user == null) throw new Exception("No default system user found");

				TalkChannel channel = new TalkChannel
				{
					Id = Guid.NewGuid(),
					Name = "Test Channel",
					DataIdentity = TfConstants.TF_ROW_ID_DATA_IDENTITY,
					CountSharedColumnName = "sc_int_row_id"
				};
				var channelCreated = talkService.CreateChannel(channel);

				CreateTalkThreadWithRowIdModel thread = new CreateTalkThreadWithRowIdModel
				{
					ChannelId = channel.Id,
					Content = "content",
					Type = TalkThreadType.Comment,
					UserId = user.Id,
					RowIds = rowIds,
					DataProviderId = provider.Id
				};

				var createdThread1 = talkService.CreateThread(thread);

				var th = talkService.GetThread(createdThread1.Id);
				th.Should().NotBeNull();
				th.ConnectedDataIdentityValuesCount.Should().Be(rowIds.Count);

				var connectedIdentityValues = talkService.GetThreadRelatedIdentityValues(th);
				connectedIdentityValues.Count.Should().Be(rowIds.Count);

				var threads = talkService.GetThreads(channel.Id, dataIdentityValue: null);
				threads.Count.Should().Be(1);


				var firstDataIdentityValue = talkService.GetThreadRelatedIdentityValues(th).First();

				threads = talkService.GetThreads(channel.Id, dataIdentityValue: firstDataIdentityValue);
				threads.Count.Should().Be(1);

				CreateTalkSubThread thread2 = new CreateTalkSubThread
				{
					ThreadId = createdThread1.Id,
					Content = "sub thread content2",
					UserId = user.Id,
				};

				var createdThread2 = talkService.CreateSubThread(thread2);
				threads = talkService.GetThreads(channel.Id, firstDataIdentityValue);
				threads.Count.Should().Be(1);
				threads[0].SubThread.Count.Should().Be(1);

				CreateTalkSubThread thread3 = new CreateTalkSubThread
				{
					ThreadId = createdThread1.Id,
					Content = "sub thread content3",
					UserId = user.Id,
					VisibleInChannel = true
				};

				var createdThread3 = talkService.CreateSubThread(thread3);
				threads = talkService.GetThreads(channel.Id, firstDataIdentityValue);
				threads.Count.Should().Be(2);
			}
		}
	}

	[Fact]
	public async Task Talk_Performance_Test()
	{
		using (await locker.LockAsync())
		{
			//ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			//ITalkService talkService = ServiceProvider.GetRequiredService<ITalkService>();
			//ITfService tfService = ServiceProvider.GetService<ITfService>();


			//using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			//{
			//	var (provider, spaceData) = await CreateTestStructureAndData(ServiceProvider, dbService);
			//	var dataTable = tfService.QueryDataProvider(provider);

			//	var user = tfService.GetDefaultSystemUser();

			//	TalkChannel channel = new TalkChannel
			//	{
			//		Id = Guid.NewGuid(),
			//		Name = "Test Channel",
			//		DataIdentity = TfConstants.TF_ROW_ID_DATA_IDENTITY,
			//		CountSharedColumnName = "sc_int_row_id"
			//	};

			//	channel = talkService.CreateChannel(channel);
			//	channel.Should().NotBeNull();

			//	channel.Name = "Test channel 1";
			//	channel = talkService.UpdateChannel(channel);
			//	channel.Name.Should().Be("Test channel 1");


			//	//try to create channel with same name 
			//	//should fail with validation exception
			//	Exception exception = null;
			//	try { talkService.CreateChannel(channel with { Id = Guid.NewGuid() }); } catch (Exception ex) { exception = ex; }
			//	exception.Should().NotBeNull();
			//	exception.Should().BeOfType(typeof(TfValidationException));

			//	List<Guid> rowIdentityIds = new List<Guid>();
			//	for (int i = 0; i < 10000; i++)
			//		rowIdentityIds.Add((Guid)dataTable.Rows[i]["tf_id"]);

			//	for (int i = 0; i<1000; i++)
			//	{
			//		CreateTalkThreadWithRowIdModel thread = new CreateTalkThreadWithRowIdModel
			//		{
			//			ChannelId = channel.Id,
			//			Content = "content" + i,
			//			Type = TalkThreadType.Comment,
			//			UserId = user.Id,
			//			RowIds = rowIdentityIds,
			//			DataProviderId = provider.Id
			//		};
			//		var createdThread1 = talkService.CreateThread(thread);
			//	}

			//	scope.Complete();
			//}
		}
	}
}
