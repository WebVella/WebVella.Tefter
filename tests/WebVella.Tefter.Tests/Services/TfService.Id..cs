using Bogus;

namespace WebVella.Tefter.Tests.Services;

public partial class TfServiceTest : BaseTest
{
	[Fact]
	public async Task Id_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				Guid? id1 = null;
				Guid? id2 = null;

				var task = Task.Run(() => { id1 = tfService.GetId("test"); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				id1.Should().NotBeNull();

				task = Task.Run(() => { id2 = tfService.GetId("test"); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				id2.Should().NotBeNull();

				id1.Should().Be(id2.Value);
			}
		}
	}


	[Fact]
	public async Task Id_BuildFill()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var faker = new Faker("en");

				Dictionary<string, Guid> idsDict = new Dictionary<string, Guid>();

				for (int i = 0; i < 10000; i++)
				{
					List<string> textList = new List<string>();
					textList.Add(faker.Random.ReplaceNumbers("########################################"));
					textList.Add(faker.Random.ReplaceNumbers("########################################"));
					textList.Add(faker.Random.ReplaceNumbers("########################################"));

					var combinedKey = tfService.CombineKey(textList);
					idsDict.Add(combinedKey, Guid.Empty);
				}

				var task = Task.Run(() => { tfService.BulkFillIds(idsDict); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				foreach (var guid in idsDict.Values)
				{
					guid.Should().NotBeEmpty();
				}

			}
		}
	}

}