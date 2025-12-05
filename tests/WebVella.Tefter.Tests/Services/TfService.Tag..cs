using WebVella.Tefter.Models;

namespace WebVella.Tefter.Tests.Services;

public partial class TfServiceTest : BaseTest
{
	[Fact]
	public async Task Tag_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>()!;

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var input = new TfTag(){ Id = Guid.NewGuid(), Label = Guid.NewGuid().ToString()};
				TfTag? result = null;

				var task = Task.Run(() => { result = tfService.CreateTag(input); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				result.Should().NotBeNull();
				result.Id.Should().Be(input.Id);
				result.Label.Should().Be(input.Label);

				input.Label = Guid.NewGuid().ToString();
				task = Task.Run(() => { result = tfService.UpdateTag(input); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				result.Should().NotBeNull();
				result.Id.Should().Be(input.Id);
				result.Label.Should().Be(input.Label);

				task = Task.Run(() => { tfService.DeleteTag(input.Id); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				task = Task.Run(() => { result = tfService.GetTag(input.Id); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				result.Should().BeNull();

				task = Task.Run(() => { result = tfService.GetTag(input.Label); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				result.Should().BeNull();
			}
		}
	}

}
