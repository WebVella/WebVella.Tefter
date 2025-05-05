namespace WebVella.Tefter.Tests.Services;

using WebVella.Tefter.Models;
using WebVella.Tefter.Services;
using WebVella.Tefter.TemplateProcessors.TextContent;
using WebVella.Tefter.TemplateProcessors.TextContent.Addons;

public partial class TfServiceTest : BaseTest
{

	[Fact]
	public async Task Setting_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				TfSetting setting = new TfSetting
				{
					Key = "unit.test.setting",
					Value = "unit test value",
				};

				var task = Task.Run(() => { tfService.SaveSetting(setting); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				TfSetting createdSetting = null;
				task = Task.Run(() => { createdSetting = tfService.GetSetting(setting.Key); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				createdSetting.Should().NotBeNull();
				createdSetting.Key.Should().Be(setting.Key);
				createdSetting.Value.Should().Be(setting.Value);

				List<TfSetting> allSettings = null;
				task = Task.Run(() => { allSettings = tfService.GetSettings(); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				allSettings.Should().NotBeNull();
				allSettings.Count.Should().Be(1);
				allSettings[0].Key.Should().Be(setting.Key);
				allSettings[0].Value.Should().Be(setting.Value);

				setting.Value = "unit test value 2";
				task = Task.Run(() => { tfService.SaveSetting(setting); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();


				TfSetting updatedSetting = null;
				task = Task.Run(() => { updatedSetting = tfService.GetSetting(setting.Key); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				updatedSetting.Should().NotBeNull();
				updatedSetting.Key.Should().Be(setting.Key);
				updatedSetting.Value.Should().Be(setting.Value);


				task = Task.Run(() => { tfService.DeleteSetting(setting.Key); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				
				task = Task.Run(() => { allSettings = tfService.GetSettings(); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				allSettings.Should().NotBeNull();
				allSettings.Count.Should().Be(0);

				TfSetting deletedSetting = null;
				task = Task.Run(() => { deletedSetting = tfService.GetSetting(setting.Key); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				deletedSetting.Should().BeNull();
			}
		}
	}
}
