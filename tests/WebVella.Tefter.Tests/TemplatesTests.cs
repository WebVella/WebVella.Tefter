namespace WebVella.Tefter.Tests;

using WebVella.Tefter.Templates.Models;
using WebVella.Tefter.Templates.Services;
using WebVella.Tefter.Templates.ContentProcessors;


public partial class TemplatesTests : BaseTest
{
	[Fact]
	public async Task Templates_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITemplatesService templatesService = ServiceProvider.GetRequiredService<ITemplatesService>();
			IIdentityManager identityManager = ServiceProvider.GetRequiredService<IIdentityManager>();
			ITfDataManager dataManager = ServiceProvider.GetRequiredService<ITfDataManager>();


			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var user = identityManager.GetUser("rumen@webvella.com").Value;

				CreateTemplateModel createTemplateModel = new CreateTemplateModel
				{
					Name = "unit test",
					Description = "unit test desc",
					Icon = string.Empty,
					ContentProcessorType = typeof(FileContentProcessor),
					IsEnabled = true,
					IsSelectable = true,
					ResultType = TemplateResultType.File,
					SettingsJson = "{}",
					UserId = user.Id,
				};

				var createResult = templatesService.CreateTemplate(createTemplateModel);
				createResult.IsSuccess.Should().BeTrue();

				var allTemplateResult = templatesService.GetTemplates();
				allTemplateResult.IsSuccess.Should().BeTrue();
				allTemplateResult.Value.Count.Should().Be(1);
				allTemplateResult.Value[0].Name.Should().Be(createTemplateModel.Name);
				allTemplateResult.Value[0].Description.Should().Be(createTemplateModel.Description);
				allTemplateResult.Value[0].Icon.Should().Be(createTemplateModel.Icon);


				UpdateTemplateModel updateTemplateModel = new UpdateTemplateModel
				{
					Id = allTemplateResult.Value[0].Id,
					Name = "unit test updated",
					Description = "unit test desc updated",
					Icon = "icon",
					ContentProcessorType = typeof(FileContentProcessor),
					IsEnabled = true,
					IsSelectable = true,
					ResultType = TemplateResultType.File,
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
				allTemplateResult.Value[0].Icon.Should().Be(updateTemplateModel.Icon);

				var deleteResult = templatesService.DeleteTemplate(updateTemplateModel.Id);
				updateResult.IsSuccess.Should().BeTrue();

				allTemplateResult = templatesService.GetTemplates();
				allTemplateResult.IsSuccess.Should().BeTrue();
				allTemplateResult.Value.Count.Should().Be(0);
			}
		}
	}
}
