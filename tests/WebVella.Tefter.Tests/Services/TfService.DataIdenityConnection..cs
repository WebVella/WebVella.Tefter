using Bogus;
using System;
using WebVella.Tefter.Models;
using WebVella.Tefter.Tests.Applications;
using WebVella.Tefter.Utility;

namespace WebVella.Tefter.Tests.Services;

public partial class TfServiceTest : BaseTest
{
	[Fact]
	public async Task DataIdentityConnection_GetCreateDelete()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				TfDataIdentity dataIdentityModel1 = new TfDataIdentity
				{
					DataIdentity = "test_data_identity_1",
					Label = "Test Data Identity 1",
				};
				TfDataIdentity dataIdentity1 = tfService.CreateDataIdentity(dataIdentityModel1);
				dataIdentity1.Should().NotBeNull();

				TfDataIdentity dataIdentityModel2 = new TfDataIdentity
				{
					DataIdentity = "test_data_identity_2",
					Label = "Test Data Identity 2",
				};
				TfDataIdentity dataIdentity2 = tfService.CreateDataIdentity(dataIdentityModel2);
				dataIdentity2.Should().NotBeNull();


				List<TfDataIdentityConnection> connections = null;
				var task = Task.Run(() => { connections = tfService.GetDataIdentityConnections(); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				connections.Should().NotBeNull();
				connections.Count.Should().Be(0);

				TfDataIdentityConnection connectionModel = new TfDataIdentityConnection
				{
					DataIdentity1 = dataIdentity1.DataIdentity,
					Value1 = "8fcd4d52d4f57b72761d5198abf9946b9c27f050",
					DataIdentity2 = dataIdentity2.DataIdentity,
					Value2 = "e3ca24d9ff05b5ea06e25c34e58b609c366ba3c6"
				};

				task = Task.Run(() => { tfService.CreateDataIdentityConnection(connectionModel); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				//try to find with switched identities and values
				bool found = false;
				task = Task.Run(() =>
				{
					found = tfService.DataIdentityConnectionExists(new TfDataIdentityConnection
					{
						DataIdentity1 = connectionModel.DataIdentity2,
						Value1 = connectionModel.Value2,
						DataIdentity2 = connectionModel.DataIdentity1,
						Value2 = connectionModel.Value1
					});
				});
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				found.Should().BeTrue();

				task = Task.Run(() => { connections = tfService.GetDataIdentityConnections(); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				connections.Should().NotBeNull();
				connections.Count.Should().Be(1);

				task = Task.Run(() => { tfService.DeleteDataIdentityConnection(connectionModel); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				task = Task.Run(() => { connections = tfService.GetDataIdentityConnections(); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				connections.Should().NotBeNull();
				connections.Count.Should().Be(0);

				//create again and try to delete with switched identities and values
				task = Task.Run(() => { tfService.CreateDataIdentityConnection(connectionModel); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				task = Task.Run(() => { tfService.DeleteDataIdentityConnection(new TfDataIdentityConnection
				{
					DataIdentity1 = connectionModel.DataIdentity2,
					Value1 = connectionModel.Value2,
					DataIdentity2 = connectionModel.DataIdentity1,
					Value2 = connectionModel.Value1
				}); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				task = Task.Run(() => { connections = tfService.GetDataIdentityConnections(); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				connections.Should().NotBeNull();
				connections.Count.Should().Be(0);

			}
		}
	}

	[Fact]
	public async Task DataIdentityConnection_TryCreateAlreadyExistingConnection()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				TfDataIdentity dataIdentityModel1 = new TfDataIdentity
				{
					DataIdentity = "test_data_identity_1",
					Label = "Test Data Identity 1",
				};
				TfDataIdentity dataIdentity1 = tfService.CreateDataIdentity(dataIdentityModel1);
				dataIdentity1.Should().NotBeNull();

				TfDataIdentity dataIdentityModel2 = new TfDataIdentity
				{
					DataIdentity = "test_data_identity_2",
					Label = "Test Data Identity 2",
				};
				TfDataIdentity dataIdentity2 = tfService.CreateDataIdentity(dataIdentityModel2);
				dataIdentity2.Should().NotBeNull();

				TfDataIdentityConnection connectionModel = new TfDataIdentityConnection
				{
					DataIdentity1 = dataIdentity1.DataIdentity,
					Value1 = "8fcd4d52d4f57b72761d5198abf9946b9c27f050",
					DataIdentity2 = dataIdentity2.DataIdentity,
					Value2 = "e3ca24d9ff05b5ea06e25c34e58b609c366ba3c6"
				};

				var task = Task.Run(() => { tfService.CreateDataIdentityConnection(connectionModel); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				task = Task.Run(() => { tfService.CreateDataIdentityConnection(connectionModel); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains(string.Empty).Should().BeTrue();

				//try to create same identity with switched identities and values
				task = Task.Run(() =>
				{
					tfService.CreateDataIdentityConnection(new TfDataIdentityConnection
					{
						DataIdentity1 = connectionModel.DataIdentity2,
						Value1 = connectionModel.Value2,
						DataIdentity2 = connectionModel.DataIdentity1,
						Value2 = connectionModel.Value1
					});
				});
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
			}
		}
	}

	[Fact]
	public async Task DataIdentityConnection_TryDeleteNonExistingConnection()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				TfDataIdentity dataIdentityModel1 = new TfDataIdentity
				{
					DataIdentity = "test_data_identity_1",
					Label = "Test Data Identity 1",
				};
				TfDataIdentity dataIdentity1 = tfService.CreateDataIdentity(dataIdentityModel1);
				dataIdentity1.Should().NotBeNull();

				TfDataIdentity dataIdentityModel2 = new TfDataIdentity
				{
					DataIdentity = "test_data_identity_2",
					Label = "Test Data Identity 2",
				};
				TfDataIdentity dataIdentity2 = tfService.CreateDataIdentity(dataIdentityModel2);
				dataIdentity2.Should().NotBeNull();

				TfDataIdentityConnection connectionModel = new TfDataIdentityConnection
				{
					DataIdentity1 = dataIdentity1.DataIdentity,
					Value1 = "8fcd4d52d4f57b72761d5198abf9946b9c27f050",
					DataIdentity2 = dataIdentity2.DataIdentity,
					Value2 = "e3ca24d9ff05b5ea06e25c34e58b609c366ba3c6"
				};

				var task = Task.Run(() => { tfService.CreateDataIdentityConnection(connectionModel); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				//change last char from 0 to 1
				connectionModel.Value1 = "8fcd4d52d4f57b72761d5198abf9946b9c27f051";
				task = Task.Run(() => { tfService.DeleteDataIdentityConnection(connectionModel); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains(string.Empty).Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task DataIdentityConnection_TryCreateWithInvalidIdentities()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				TfDataIdentity dataIdentityModel1 = new TfDataIdentity
				{
					DataIdentity = "test_data_identity_1",
					Label = "Test Data Identity 1",
				};
				TfDataIdentity dataIdentity1 = tfService.CreateDataIdentity(dataIdentityModel1);
				dataIdentity1.Should().NotBeNull();

				TfDataIdentity dataIdentityModel2 = new TfDataIdentity
				{
					DataIdentity = "test_data_identity_2",
					Label = "Test Data Identity 2",
				};
				TfDataIdentity dataIdentity2 = tfService.CreateDataIdentity(dataIdentityModel2);
				dataIdentity2.Should().NotBeNull();

				TfDataIdentityConnection connectionModel = new TfDataIdentityConnection
				{
					DataIdentity1 = "non_existant_source_identity",
					Value1 = "8fcd4d52d4f57b72761d5198abf9946b9c27f050",
					DataIdentity2 = dataIdentity2.DataIdentity,
					Value2 = "e3ca24d9ff05b5ea06e25c34e58b609c366ba3c6"
				};

				var task = Task.Run(() => { tfService.CreateDataIdentityConnection(connectionModel); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains(nameof(TfDataIdentityConnection.DataIdentity1)).Should().BeTrue();

				connectionModel.DataIdentity1 = dataIdentity1.DataIdentity;
				connectionModel.DataIdentity2 = "non_existant_target_identity";
				task = Task.Run(() => { tfService.CreateDataIdentityConnection(connectionModel); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains(nameof(TfDataIdentityConnection.DataIdentity2)).Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task DataIdentityConnection_TryCreateWithInvalidValues()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				TfDataIdentity dataIdentityModel1 = new TfDataIdentity
				{
					DataIdentity = "test_data_identity_1",
					Label = "Test Data Identity 1",
				};
				TfDataIdentity dataIdentity1 = tfService.CreateDataIdentity(dataIdentityModel1);
				dataIdentity1.Should().NotBeNull();

				TfDataIdentity dataIdentityModel2 = new TfDataIdentity
				{
					DataIdentity = "test_data_identity_2",
					Label = "Test Data Identity 2",
				};
				TfDataIdentity dataIdentity2 = tfService.CreateDataIdentity(dataIdentityModel2);
				dataIdentity2.Should().NotBeNull();

				TfDataIdentityConnection connectionModel = new TfDataIdentityConnection
				{
					DataIdentity1 = dataIdentity1.DataIdentity,
					Value1 = "8fcd4d52d4f57b72761d5198abf9946b9c27f050",
					DataIdentity2 = dataIdentity2.DataIdentity,
					Value2 = "e3ca24d9ff05b5ea06e25c34e58b609c366ba3c6"
				};

				//invalid SHA1 value
				connectionModel.Value1 = "asdfasdfdasfas";
				var task = Task.Run(() => { tfService.CreateDataIdentityConnection(connectionModel); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains(nameof(TfDataIdentityConnection.Value1)).Should().BeTrue();

				connectionModel.Value1 = "8fcd4d52d4f57b72761d5198abf9946b9c27f050";
				connectionModel.Value2 = "sdafasdfasdfsda";
				task = Task.Run(() => { tfService.CreateDataIdentityConnection(connectionModel); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains(nameof(TfDataIdentityConnection.Value2)).Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task DataIdentityConnection_TestGetFilters()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				TfDataIdentity dataIdentityModel1 = new TfDataIdentity
				{
					DataIdentity = "test_data_identity_1",
					Label = "Test Data Identity 1",
				};
				TfDataIdentity dataIdentity1 = tfService.CreateDataIdentity(dataIdentityModel1);
				dataIdentity1.Should().NotBeNull();

				TfDataIdentity dataIdentityModel2 = new TfDataIdentity
				{
					DataIdentity = "test_data_identity_2",
					Label = "Test Data Identity 2",
				};
				TfDataIdentity dataIdentity2 = tfService.CreateDataIdentity(dataIdentityModel2);
				dataIdentity2.Should().NotBeNull();

				TfDataIdentity dataIdentityModel3 = new TfDataIdentity
				{
					DataIdentity = "test_data_identity_3",
					Label = "Test Data Identity 3",
				};
				TfDataIdentity dataIdentity3 = tfService.CreateDataIdentity(dataIdentityModel3);
				dataIdentity2.Should().NotBeNull();


				List<TfDataIdentityConnection> connections = null;
				var task = Task.Run(() => { connections = tfService.GetDataIdentityConnections(); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				connections.Should().NotBeNull();
				connections.Count.Should().Be(0);

				TfDataIdentityConnection connectionModel1 = new TfDataIdentityConnection
				{
					DataIdentity1 = dataIdentity1.DataIdentity,
					Value1 = "8fcd4d52d4f57b72761d5198abf9946b9c27f050",
					DataIdentity2 = dataIdentity2.DataIdentity,
					Value2 = "e3ca24d9ff05b5ea06e25c34e58b609c366ba3c6"
				};

				task = Task.Run(() => { tfService.CreateDataIdentityConnection(connectionModel1); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				TfDataIdentityConnection connectionModel2 = new TfDataIdentityConnection
				{
					DataIdentity1 = dataIdentity2.DataIdentity,
					Value1 = "8fcd4d52d4f57b72761d5198abf9946b9c27f050",
					DataIdentity2 = dataIdentity3.DataIdentity,
					Value2 = "e3ca24d9ff05b5ea06e25c34e58b609c366ba3c6"
				};

				task = Task.Run(() => { tfService.CreateDataIdentityConnection(connectionModel2); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();


				TfDataIdentityConnection connectionModel3 = new TfDataIdentityConnection
				{
					DataIdentity1 = dataIdentity1.DataIdentity,
					Value1 = "8fcd4d52d4f57b72761d5198abf9946b9c27f052",
					DataIdentity2 = dataIdentity3.DataIdentity,
					Value2 = "e3ca24d9ff05b5ea06e25c34e58b609c366ba3c7"
				};

				task = Task.Run(() => { tfService.CreateDataIdentityConnection(connectionModel3); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();


				task = Task.Run(() => { connections = tfService.GetDataIdentityConnections(); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				connections.Should().NotBeNull();
				connections.Count.Should().Be(3);


				task = Task.Run(() => { connections = tfService.GetDataIdentityConnections(dataIndentity1: dataIdentity1.DataIdentity); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				connections.Should().NotBeNull();
				connections.Count.Should().Be(2);

				task = Task.Run(() => { connections = tfService.GetDataIdentityConnections(dataIndentity1: dataIdentity2.DataIdentity); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				connections.Should().NotBeNull();
				connections.Count.Should().Be(2);

				task = Task.Run(() => { connections = tfService.GetDataIdentityConnections(dataIndentity1: dataIdentity3.DataIdentity); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				connections.Should().NotBeNull();
				connections.Count.Should().Be(2);

			

				task = Task.Run(() => { connections = tfService.GetDataIdentityConnections(value1: connectionModel1.Value2); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				connections.Should().NotBeNull();
				connections.Count.Should().Be(2);

				task = Task.Run(() => { connections = tfService.GetDataIdentityConnections(value1: connectionModel3.Value2); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				connections.Should().NotBeNull();
				connections.Count.Should().Be(1);

				task = Task.Run(() => { connections = tfService.GetDataIdentityConnections(value1: connectionModel1.Value1); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				connections.Should().NotBeNull();
				connections.Count.Should().Be(2);
			}
		}
	}

	[Fact]
	public async Task DataIdentityConnection_TestBatchOperations()
	{
		const int BATCH_SIZE = 1000;

		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				TfDataIdentity dataIdentityModel1 = new TfDataIdentity
				{
					DataIdentity = "test_data_identity_1",
					Label = "Test Data Identity 1",
				};
				TfDataIdentity dataIdentity1 = tfService.CreateDataIdentity(dataIdentityModel1);
				dataIdentity1.Should().NotBeNull();

				TfDataIdentity dataIdentityModel2 = new TfDataIdentity
				{
					DataIdentity = "test_data_identity_2",
					Label = "Test Data Identity 2",
				};
				TfDataIdentity dataIdentity2 = tfService.CreateDataIdentity(dataIdentityModel2);
				dataIdentity2.Should().NotBeNull();

				List<TfDataIdentityConnection> connections = new();

				for(int i= 0; i < BATCH_SIZE*20; i++)
				{
					connections.Add(new TfDataIdentityConnection
					{
						DataIdentity1 = dataIdentity1.DataIdentity,
						Value1 = (dataIdentity1.DataIdentity + i.ToString()).ToSha1(),
						DataIdentity2 = dataIdentity2.DataIdentity,
						Value2 = (dataIdentity2.DataIdentity + i.ToString()).ToSha1()
					});
				}

				var task = Task.Run(() => { tfService.CreateBatchDataIdentityConnections(connections); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				var existingInDatabaseIndentityConnections = tfService.GetDataIdentityConnections();
				existingInDatabaseIndentityConnections.Count.Should().Be(connections.Count);
				
				//foreach(var conn in connections)
				//{
				//	existingInDatabaseIndentityConnections.Any(x =>
				//		x.DataIdentity1 == conn.DataIdentity1 && x.Value1 == conn.Value1
				//		&& x.DataIdentity2 == conn.DataIdentity2 && x.Value2 == conn.Value2
				//	).Should().BeTrue();
				//}


				task = Task.Run(() => { tfService.DeleteBatchDataIdentityConnections(connections); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				existingInDatabaseIndentityConnections = tfService.GetDataIdentityConnections();
				existingInDatabaseIndentityConnections.Count.Should().Be(0);

			}
		}
	}

}