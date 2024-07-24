using WebVella.Tefter.Api;
using WebVella.Tefter.Web.Services;

namespace WebVella.Tefter.Tests.DataProviders;

public partial class TfSharedColumnsManagerTests : BaseTest
{
	[Fact]
	public async Task SharedColumn_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfSharedColumnsManager sharedColumnManager = ServiceProvider.GetRequiredService<ITfSharedColumnsManager>();
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				TfSharedColumn sharedColumn = new TfSharedColumn
				{
					Id = Guid.NewGuid(),
					AddonId = null,
					DbName = "sk_test",
					DbType = DatabaseColumnType.Text,
					IncludeInTableSearch = false,
					SharedKeyDbName = "shared_key"
				};
				var result = sharedColumnManager.CreateSharedColumn(sharedColumn);
				result.IsSuccess.Should().BeTrue();

				var sharedColumnsResult =sharedColumnManager.GetSharedColumns();
				sharedColumnsResult.IsSuccess.Should().BeTrue();

				var sharedColumns = sharedColumnsResult.Value;
				sharedColumns.Count().Should().Be(1);
				sharedColumns[0].Id.Should().Be(sharedColumn.Id);
				sharedColumns[0].AddonId.Should().Be(sharedColumn.AddonId);
				sharedColumns[0].DbName.Should().Be(sharedColumn.DbName);
				sharedColumns[0].DbType.Should().Be(sharedColumn.DbType);
				sharedColumns[0].IncludeInTableSearch.Should().Be(sharedColumn.IncludeInTableSearch);
				sharedColumns[0].SharedKeyDbName.Should().Be(sharedColumn.SharedKeyDbName);

				sharedColumn.AddonId = Guid.NewGuid();
				sharedColumn.DbName = "sk_test1";
				sharedColumn.DbType = DatabaseColumnType.Integer;
				sharedColumn.IncludeInTableSearch = !sharedColumn.IncludeInTableSearch;
				sharedColumn.SharedKeyDbName = "shared_key_1";

				result = sharedColumnManager.UpdateSharedColumn(sharedColumn);
				result.IsSuccess.Should().BeTrue();

				sharedColumnsResult = sharedColumnManager.GetSharedColumns();
				sharedColumnsResult.IsSuccess.Should().BeTrue();

				sharedColumns = sharedColumnsResult.Value;
				sharedColumns.Count().Should().Be(1);
				sharedColumns[0].Id.Should().Be(sharedColumn.Id);
				sharedColumns[0].AddonId.Should().Be(sharedColumn.AddonId);
				sharedColumns[0].DbName.Should().Be(sharedColumn.DbName);
				sharedColumns[0].DbType.Should().Be(sharedColumn.DbType);
				sharedColumns[0].IncludeInTableSearch.Should().Be(sharedColumn.IncludeInTableSearch);
				sharedColumns[0].SharedKeyDbName.Should().Be(sharedColumn.SharedKeyDbName);

				result = sharedColumnManager.DeleteSharedColumn(sharedColumn.Id);
				result.IsSuccess.Should().BeTrue();

				sharedColumnsResult = sharedColumnManager.GetSharedColumns();
				sharedColumnsResult.IsSuccess.Should().BeTrue();

				sharedColumns = sharedColumnsResult.Value;
				sharedColumns.Count().Should().Be(0);
			}
		}
	}
}
