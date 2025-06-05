using Bogus;
using System;
using WebVella.Tefter.Models;
using WebVella.Tefter.Tests.Applications;

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
					SourceDataIdentity = dataIdentity1.DataIdentity,
					SourceDataValue = "8fcd4d52d4f57b72761d5198abf9946b9c27f050",
					TargetDataIdentity = dataIdentity2.DataIdentity,
					TargetDataValue = "e3ca24d9ff05b5ea06e25c34e58b609c366ba3c6"
				};

				task = Task.Run(() => { tfService.CreateDataIdentityConnection(connectionModel); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

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
					SourceDataIdentity = dataIdentity1.DataIdentity,
					SourceDataValue = "8fcd4d52d4f57b72761d5198abf9946b9c27f050",
					TargetDataIdentity = dataIdentity2.DataIdentity,
					TargetDataValue = "e3ca24d9ff05b5ea06e25c34e58b609c366ba3c6"
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
					SourceDataIdentity = dataIdentity1.DataIdentity,
					SourceDataValue = "8fcd4d52d4f57b72761d5198abf9946b9c27f050",
					TargetDataIdentity = dataIdentity2.DataIdentity,
					TargetDataValue = "e3ca24d9ff05b5ea06e25c34e58b609c366ba3c6"
				};

				var task = Task.Run(() => { tfService.CreateDataIdentityConnection(connectionModel); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				//change last char from 0 to 1
				connectionModel.SourceDataValue = "8fcd4d52d4f57b72761d5198abf9946b9c27f051"; 
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
					SourceDataIdentity = "non_existant_source_identity",
					SourceDataValue = "8fcd4d52d4f57b72761d5198abf9946b9c27f050",
					TargetDataIdentity = dataIdentity2.DataIdentity,
					TargetDataValue = "e3ca24d9ff05b5ea06e25c34e58b609c366ba3c6"
				};

				var task = Task.Run(() => { tfService.CreateDataIdentityConnection(connectionModel); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains(nameof(TfDataIdentityConnection.SourceDataIdentity)).Should().BeTrue();

				connectionModel.SourceDataIdentity = dataIdentity1.DataIdentity;
				connectionModel.TargetDataIdentity = "non_existant_target_identity";
				task = Task.Run(() => { tfService.CreateDataIdentityConnection(connectionModel); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains(nameof(TfDataIdentityConnection.TargetDataIdentity)).Should().BeTrue();
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
					SourceDataIdentity = dataIdentity1.DataIdentity,
					SourceDataValue = "8fcd4d52d4f57b72761d5198abf9946b9c27f050",
					TargetDataIdentity = dataIdentity2.DataIdentity,
					TargetDataValue = "e3ca24d9ff05b5ea06e25c34e58b609c366ba3c6"
				};

				//invalid SHA1 value
				connectionModel.SourceDataValue = "asdfasdfdasfas";
				var task = Task.Run(() => { tfService.CreateDataIdentityConnection(connectionModel); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains(nameof(TfDataIdentityConnection.SourceDataValue)).Should().BeTrue();

				connectionModel.SourceDataValue = "8fcd4d52d4f57b72761d5198abf9946b9c27f050";
				connectionModel.TargetDataValue = "sdafasdfasdfsda";
				task = Task.Run(() => { tfService.CreateDataIdentityConnection(connectionModel); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains(nameof(TfDataIdentityConnection.TargetDataValue)).Should().BeTrue();
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
					SourceDataIdentity = dataIdentity1.DataIdentity,
					SourceDataValue = "8fcd4d52d4f57b72761d5198abf9946b9c27f050",
					TargetDataIdentity = dataIdentity2.DataIdentity,
					TargetDataValue = "e3ca24d9ff05b5ea06e25c34e58b609c366ba3c6"
				};

				task = Task.Run(() => { tfService.CreateDataIdentityConnection(connectionModel1); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				TfDataIdentityConnection connectionModel2 = new TfDataIdentityConnection
				{
					SourceDataIdentity = dataIdentity2.DataIdentity,
					SourceDataValue = "8fcd4d52d4f57b72761d5198abf9946b9c27f050",
					TargetDataIdentity = dataIdentity3.DataIdentity,
					TargetDataValue = "e3ca24d9ff05b5ea06e25c34e58b609c366ba3c6"
				};

				task = Task.Run(() => { tfService.CreateDataIdentityConnection(connectionModel2); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();


				TfDataIdentityConnection connectionModel3 = new TfDataIdentityConnection
				{
					SourceDataIdentity = dataIdentity1.DataIdentity,
					SourceDataValue = "8fcd4d52d4f57b72761d5198abf9946b9c27f052",
					TargetDataIdentity = dataIdentity3.DataIdentity,
					TargetDataValue = "e3ca24d9ff05b5ea06e25c34e58b609c366ba3c7"
				};

				task = Task.Run(() => { tfService.CreateDataIdentityConnection(connectionModel3); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();


				task = Task.Run(() => { connections = tfService.GetDataIdentityConnections(); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				connections.Should().NotBeNull();
				connections.Count.Should().Be(3);


				task = Task.Run(() => { connections = tfService.GetDataIdentityConnections(sourceDataIndentity: dataIdentity1.DataIdentity); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				connections.Should().NotBeNull();
				connections.Count.Should().Be(2);

				task = Task.Run(() => { connections = tfService.GetDataIdentityConnections(sourceDataIndentity: dataIdentity2.DataIdentity); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				connections.Should().NotBeNull();
				connections.Count.Should().Be(1);

				task = Task.Run(() => { connections = tfService.GetDataIdentityConnections(sourceDataIndentity: dataIdentity3.DataIdentity); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				connections.Should().NotBeNull();
				connections.Count.Should().Be(0);

				task = Task.Run(() => { connections = tfService.GetDataIdentityConnections(targetDataIdentity: dataIdentity1.DataIdentity); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				connections.Should().NotBeNull();
				connections.Count.Should().Be(0);

				task = Task.Run(() => { connections = tfService.GetDataIdentityConnections(targetDataIdentity: dataIdentity2.DataIdentity); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				connections.Should().NotBeNull();
				connections.Count.Should().Be(1);

				task = Task.Run(() => { connections = tfService.GetDataIdentityConnections(targetDataIdentity: dataIdentity3.DataIdentity); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				connections.Should().NotBeNull();
				connections.Count.Should().Be(2);


				task = Task.Run(() => { connections = tfService.GetDataIdentityConnections(targetDataValue: connectionModel1.TargetDataValue); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				connections.Should().NotBeNull();
				connections.Count.Should().Be(2);

				task = Task.Run(() => { connections = tfService.GetDataIdentityConnections(targetDataValue: connectionModel3.TargetDataValue); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				connections.Should().NotBeNull();
				connections.Count.Should().Be(1);

				task = Task.Run(() => { connections = tfService.GetDataIdentityConnections(sourceDataValue: connectionModel1.SourceDataValue); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				connections.Should().NotBeNull();
				connections.Count.Should().Be(2);
			}
		}
	}

}