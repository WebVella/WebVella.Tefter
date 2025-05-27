using Bogus;
using DocumentFormat.OpenXml.EMMA;
using System;
using WebVella.Tefter.Exceptions;
using WebVella.Tefter.Models;
using WebVella.Tefter.Web.Addons;

namespace WebVella.Tefter.Tests.Services;

public partial class TfServiceTest : BaseTest
{
	[Fact]
	public async Task DataIdentity_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{

				TfDataIdentity model = new TfDataIdentity
				{
					DataIdentity = "test_data_identity",
					Label = "Test Data Identity",
				};

				TfDataIdentity dataIdentity = null;
				var task = Task.Run(() => { dataIdentity  = tfService.CreateDataIdentity(model); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				dataIdentity.Should().NotBeNull();
				dataIdentity.DataIdentity.Should().Be(model.DataIdentity);
				dataIdentity.Label.Should().Be(model.Label);


				List<TfDataIdentity> dataIdentities = null;
				task = Task.Run(() => { dataIdentities = tfService.GetDataIdentities(); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				dataIdentities.Should().NotBeNull();
				dataIdentities.Count.Should().Be(2);	

				TfDataIdentity model2 = new TfDataIdentity
				{
					DataIdentity = "test_data_identity",
					Label = "Test Data Identity 2",
				};

				dataIdentity = null;
				task = Task.Run(() => { dataIdentity = tfService.UpdateDataIdentity(model2); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				dataIdentity.Should().NotBeNull();
				dataIdentity.DataIdentity.Should().Be(model2.DataIdentity);
				dataIdentity.Label.Should().Be(model2.Label);

				task = Task.Run(() => { tfService.DeleteDataIdentity(model2.DataIdentity); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				dataIdentities = null;
				task = Task.Run(() => { dataIdentities = tfService.GetDataIdentities(); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				dataIdentities.Should().NotBeNull();
				dataIdentities.Count.Should().Be(1);

			}
		}
	}

	[Fact]
	public async Task DataIdentity_TryDeleteRowIdSystemIdentity()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var task = Task.Run(() => { tfService.DeleteDataIdentity(TfConstants.TF_ROW_ID_DATA_IDENTITY); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains("DataIdentity").Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task DataIdentity_TryCreateWithInvalidDataIdentityName()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{

				TfDataIdentity model = new TfDataIdentity
				{
					DataIdentity = "test_data_identity",
					Label = "Test Data Identity",
				};

				//empty name
				model.DataIdentity = string.Empty;
				TfDataIdentity dataIdentity = null;
				var task = Task.Run(() => { dataIdentity = tfService.CreateDataIdentity(model); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains("DataIdentity").Should().BeTrue();

				//name to short
				model.DataIdentity = "a";
				task = Task.Run(() => { dataIdentity = tfService.CreateDataIdentity(model); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains("DataIdentity").Should().BeTrue();

				//name to long
				model.DataIdentity = "asdfsdf_sadfas_a_asdfsdf_sadfas_a_asdfsdf_sadfas_a_asdfsdf_sadfas_a_asdfsdf_sadfas_a_asdfsdf_sadfas_a_asdfsdf_sadfas_a_asdfsdf_sadfas_a_";
				task = Task.Run(() => { dataIdentity = tfService.CreateDataIdentity(model); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains("DataIdentity").Should().BeTrue();

				//name not apply rules
				model.DataIdentity = "asdfsdf sadfas";
				task = Task.Run(() => { dataIdentity = tfService.CreateDataIdentity(model); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains("DataIdentity").Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task DataIdentity_TryUpdateNonExistant()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{

				TfDataIdentity model = new TfDataIdentity
				{
					DataIdentity = "test_data_identity",
					Label = "Test Data Identity",
				};

				//empty name
				model.DataIdentity = string.Empty;
				TfDataIdentity dataIdentity = null;
				var task = Task.Run(() => { dataIdentity = tfService.UpdateDataIdentity(model); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains("DataIdentity").Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task DataIdentity_TryDeleteWhileUsedByProvider()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{

				TfDataIdentity model = new TfDataIdentity
				{
					DataIdentity = "test_data_identity",
					Label = "Test Data Identity",
				};

				//empty name
				model.DataIdentity = string.Empty;
				TfDataIdentity dataIdentity = null;
				var task = Task.Run(() => { dataIdentity = tfService.UpdateDataIdentity(model); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains("DataIdentity").Should().BeTrue();
			}
		}
	}

}

