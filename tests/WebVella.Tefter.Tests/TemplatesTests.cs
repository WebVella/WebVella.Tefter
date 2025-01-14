namespace WebVella.Tefter.Tests;

using WebVella.Tefter.Models;
using WebVella.Tefter.Services;
using WebVella.Tefter.TemplateProcessors.TextContent;

public partial class TemplatesTests : BaseTest
{
	[Fact]
	public async Task Dev_Debug()
	{
		//using (await locker.LockAsync())
		//{
		//	ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
		//	ITfTemplateService templatesService = ServiceProvider.GetRequiredService<ITfTemplateService>();
		//	ITfSpaceManager spaceManager = ServiceProvider.GetRequiredService<ITfSpaceManager>();
		//	ITfDataManager dataManager = ServiceProvider.GetRequiredService<ITfDataManager>();
		//	ITfBlobManager blobManager = ServiceProvider.GetRequiredService<ITfBlobManager>();
		//	blobManager.BlobStoragePath = @"\\192.168.0.190\Install\TefterBlobStorage";

		//	using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
		//	{
		//		//var templateId = new Guid("b4c24392-3276-4c69-905f-4d177906e62b"); //excel
		//		var templateId = new Guid("e59a67f9-243f-4180-b9c3-2eaea4134e9a"); //email
		//		var spaceDataId = new Guid("ff4b8789-34d4-43a2-a4da-f9bbac4950ba");
		//		//var spaceData = spaceManager.GetSpaceData(spaceDataId).Value;
		//		//var template = templatesService.GetTemplate(templateId).Value;

		//		var dataTable = dataManager.QuerySpaceData(spaceDataId, returnOnlyTfIds: true).Value;
		//		List<Guid> ids = new List<Guid>();
		//		foreach (TfDataRow row in dataTable.Rows)
		//			ids.Add((Guid)row["tf_id"]);

		//		var result = templatesService.GenerateTemplatePreviewResult(templateId, spaceDataId, ids);

		//	}
		//}
	}

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
					ContentProcessorType = typeof(TextContentTemplateProcessor),
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
					ContentProcessorType = typeof(TextContentTemplateProcessor),
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
