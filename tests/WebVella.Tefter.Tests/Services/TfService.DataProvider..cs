﻿using DocumentFormat.OpenXml.Math;
using System;
using WebVella.Tefter.Models;
using WebVella.Tefter.Services;

namespace WebVella.Tefter.Tests.Services;

public partial class TfServiceTest : BaseTest
{
	//[Fact]
	//public async Task _Provider_Lists()
	//{
	//	using (await locker.LockAsync())
	//	{
	//		ITfService tfService = ServiceProvider.GetService<ITfService>();
	//		ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

	//		using (var scope = dbService.CreateTransactionScope())
	//		{
	//			var providerTypes = tfService.GetDataProviderTypes();
	//			var providerType = providerTypes
	//				.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

	//			var provider = tfService.GetDataProvider(new Guid("b90d2c29-5600-401d-821e-b7d85c676ee5"));

	//			var dataTable = tfService.QueryDataProvider(provider, search: null, page: null, pageSize: null);
	//			var dt = dataTable.ToDataTable();

	//		}
	//	}
	//}

	private TfDataProvider CreateProviderInternal(
		ITfService tfService,
		ITfMetaService tfMetaService, 
		string name = null )
	{
		var providerTypes = tfMetaService.GetDataProviderTypes();
		var providerType = providerTypes
			.Single(x => x.AddonId == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

		Guid id = Guid.NewGuid();
		TfCreateDataProvider model = new TfCreateDataProvider
		{
			Name = name??"test data provider",
			ProviderType = providerType,
			SettingsJson = null
		};

		TfDataProvider provider = null;
		var task = Task.Run(() => { provider = tfService.CreateDataProvider(model); });
		var exception = Record.ExceptionAsync(async () => await task).Result;
		exception.Should().BeNull();
		provider.Should().NotBeNull();
		provider.Should().BeOfType<TfDataProvider>();
		return provider;
	}

	[Fact]
	public async Task DataProvider_CreateDataProvider()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				CreateProviderInternal(tfService, tfMetaService);
			}
		}
	}


	[Fact]
	public async Task DataProvider_CreateDataProviderWithSameIndex()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var providerTypes = tfMetaService.GetDataProviderTypes();
				var providerType = providerTypes
					.Single(x => x.AddonId == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				TfCreateDataProvider model = new TfCreateDataProvider
				{
					Id = Guid.NewGuid(),
					Name = "test data provider",
					ProviderType = providerType,
					SettingsJson = null,
					Index = 1
				};

				TfDataProvider provider = null;
				var task = Task.Run(() => { provider = tfService.CreateDataProvider(model); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				provider.Should().NotBeNull();
				provider.Should().BeOfType<TfDataProvider>();


				TfCreateDataProvider model2 = new TfCreateDataProvider
				{
					Id = Guid.NewGuid(),
					Name = "test data provider2",
					ProviderType = providerType,
					SettingsJson = null,
					Index = 1
				};

				task = Task.Run(() => { provider = tfService.CreateDataProvider(model2); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains("Index").Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task DataProvider_CreateDataProviderWithInvalidIndex()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var providerTypes = tfMetaService.GetDataProviderTypes();
				var providerType = providerTypes
					.Single(x => x.AddonId == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				TfCreateDataProvider model = new TfCreateDataProvider
				{
					Id = Guid.NewGuid(),
					Name = "test data provider",
					ProviderType = providerType,
					SettingsJson = null,
					Index = 0
				};

				TfDataProvider provider = null;
				var task = Task.Run(() => { provider = tfService.CreateDataProvider(model); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains("Index").Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task DataProvider_DeleteDataProvider()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateProviderInternal(tfService, tfMetaService);

				var task = Task.Run(() => { tfService.DeleteDataProvider(provider.Id); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
			}
		}
	}

	[Fact]
	public async Task DataProvider_CreateProviderWithNoProviderType()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var providerTypes = tfMetaService.GetDataProviderTypes();
				var providerType = providerTypes
					.Single(x => x.AddonId == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				Guid id = Guid.NewGuid();
				TfCreateDataProvider model = new TfCreateDataProvider
				{
					Name = "test",
					ProviderType = null,
					SettingsJson = null
				};

				TfDataProvider provider = null;
				var task = Task.Run(() => { provider = tfService.CreateDataProvider(model); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains("ProviderType").Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task DataProvider_CreateProviderWithNoName()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var providerTypes = tfMetaService.GetDataProviderTypes();
				var providerType = providerTypes
					.Single(x => x.AddonId == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				Guid id = Guid.NewGuid();
				TfCreateDataProvider model = new TfCreateDataProvider
				{
					Name = "",
					ProviderType = providerType,
					SettingsJson = null
				};

				TfDataProvider provider = null;
				var task = Task.Run(() => { provider = tfService.CreateDataProvider(model); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains("Name").Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task DataProvider_CreateProviderWithSameName()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var providerTypes = tfMetaService.GetDataProviderTypes();
				var providerType = providerTypes
					.Single(x => x.AddonId == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				Guid id = Guid.NewGuid();
				TfCreateDataProvider model = new TfCreateDataProvider
				{
					Name = "test data provider",
					ProviderType = providerType,
					SettingsJson = null
				};

				TfDataProvider provider = null;
				var task = Task.Run(() => { provider = tfService.CreateDataProvider(model); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				provider.Should().BeOfType<TfDataProvider>();

				model.Id = Guid.NewGuid();

				task = Task.Run(() => { provider = tfService.CreateDataProvider(model); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains("Name").Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task DataProvider_UpdateDataProvider()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateProviderInternal(tfService, tfMetaService);

				Guid id = Guid.NewGuid();
				TfUpdateDataProvider model = new TfUpdateDataProvider
				{
					Id = provider.Id,
					Name = "test data provider 1",
					SettingsJson = null, 
				};

				var task = Task.Run(() => { provider = tfService.UpdateDataProvider(model); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				provider.Name.Should().Be(model.Name);
			}
		}
	}

	[Fact]
	public async Task DataProvider_UpdateNonExistantDataProvider()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				Guid id = Guid.NewGuid();
				TfUpdateDataProvider model = new TfUpdateDataProvider
				{
					Id = Guid.NewGuid(),
					Name = "test data provider 1",
					SettingsJson = null,
				};

				TfDataProvider provider = null;
				var task = Task.Run(() => { provider = tfService.UpdateDataProvider(model); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains("Id").Should().BeTrue();
			}
		}
	}


	[Fact]
	public async Task DataProvider_UpdateDataProviderWithSameName()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var providerTypes = tfMetaService.GetDataProviderTypes();
				var providerType = providerTypes
					.Single(x => x.AddonId == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				TfCreateDataProvider model = new TfCreateDataProvider
				{
					Name = "test data provider",
					ProviderType = providerType,
					SettingsJson = null,
					Index = 1
				};

				TfDataProvider provider = null;
				var task = Task.Run(() => { provider = tfService.CreateDataProvider(model); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				provider.Should().BeOfType<TfDataProvider>();

				TfCreateDataProvider model2 = new TfCreateDataProvider
				{
					Name = "test data provider 2",
					ProviderType = providerType,
					SettingsJson = null,
					Index = 2
				};

				task = Task.Run(() => { provider = tfService.CreateDataProvider(model2); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				provider.Should().BeOfType<TfDataProvider>();


				TfUpdateDataProvider updateModel = new TfUpdateDataProvider
				{
					Id = model.Id,
					Name = model2.Name,
					SettingsJson = null,
				};

				task = Task.Run(() => { provider = tfService.UpdateDataProvider(updateModel); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains("Name").Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task DataProvider_UpdateDataProviderWithNoName()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateProviderInternal(tfService, tfMetaService);

				TfUpdateDataProvider model = new TfUpdateDataProvider
				{
					Id = provider.Id,
					Name = string.Empty,
					SettingsJson = null,
				};

				var task = Task.Run(() => { provider = tfService.UpdateDataProvider(model); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains("Name").Should().BeTrue();
			}
		}
	}
}
