namespace WebVella.Tefter.Tests;

using WebVella.Tefter.Models;
using WebVella.Tefter.Services;
using WebVella.Tefter.TemplateProcessors.TextContent;

public partial class TemplatesTests : BaseTest
{
	[Fact]
	public async Task Templates_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfTemplateService templatesService = ServiceProvider.GetRequiredService<ITfTemplateService>();
			IIdentityManager identityManager = ServiceProvider.GetRequiredService<IIdentityManager>();
			ITfDataManager dataManager = ServiceProvider.GetRequiredService<ITfDataManager>();


			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var user = identityManager.GetUser("rumen@webvella.com").Value;

				TfManageTemplateModel createTemplateModel = new TfManageTemplateModel
				{
					Name = "unit test",
					Description = "unit test desc",
					FluentIconName = string.Empty,
					ContentProcessorType = typeof(TextTemplateProcessor),
					IsEnabled = true,
					IsSelectable = true,
					SettingsJson = null,
					UserId = user.Id,
				};

				var createResult = templatesService.CreateTemplate(createTemplateModel);
				createResult.IsSuccess.Should().BeTrue();

				var allTemplateResult = templatesService.GetTemplates();
				allTemplateResult.IsSuccess.Should().BeTrue();
				allTemplateResult.Value.Count.Should().Be(1);
				allTemplateResult.Value[0].Name.Should().Be(createTemplateModel.Name);
				allTemplateResult.Value[0].Description.Should().Be(createTemplateModel.Description);
				allTemplateResult.Value[0].FluentIconName.Should().Be(createTemplateModel.FluentIconName);


				TfManageTemplateModel updateTemplateModel = new TfManageTemplateModel
				{
					Id = allTemplateResult.Value[0].Id,
					Name = "unit test updated",
					Description = "unit test desc updated",
					FluentIconName = "icon",
					ContentProcessorType = typeof(TextTemplateProcessor),
					IsEnabled = true,
					IsSelectable = true,
					SettingsJson = "{}",
					UserId = user.Id,
				};
				
				var updateResult = templatesService.UpdateTemplate(updateTemplateModel);
				updateResult.IsSuccess.Should().BeTrue();

				allTemplateResult = templatesService.GetTemplates();
				allTemplateResult.IsSuccess.Should().BeTrue();
				allTemplateResult.Value.Count.Should().Be(1);
				allTemplateResult.Value[0].Name.Should().Be(updateTemplateModel.Name);
				allTemplateResult.Value[0].Description.Should().Be(updateTemplateModel.Description);
				allTemplateResult.Value[0].FluentIconName.Should().Be(updateTemplateModel.FluentIconName);

				var deleteResult = templatesService.DeleteTemplate(updateTemplateModel.Id);
				deleteResult.IsSuccess.Should().BeTrue();

				allTemplateResult = templatesService.GetTemplates();
				allTemplateResult.IsSuccess.Should().BeTrue();
				allTemplateResult.Value.Count.Should().Be(0);
			}
		}
	}
}
