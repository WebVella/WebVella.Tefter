namespace WebVella.Tefter.Tests;

using WebVella.Tefter.EmailSender.Models;
using WebVella.Tefter.EmailSender.Services;


public partial class EmailSenderTests : BaseTest
{
	[Fact]
	public async Task EmailSender_Create()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			IEmailService emailService = ServiceProvider.GetRequiredService<IEmailService>();
			IIdentityManager identityManager = ServiceProvider.GetRequiredService<IIdentityManager>();
			ITfDataManager dataManager = ServiceProvider.GetRequiredService<ITfDataManager>();


			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var user = identityManager.GetUser("rumen@webvella.com").Value;
				CreateEmailMessageModel model = new CreateEmailMessageModel();
				model.Subject = "test";
				model.TextBody = "test text body";
				model.HtmlBody = "<p>test html</p>";
				model.Recipients.Add(new EmailAddress { Address = "rumen.yankov@gmail.com", Name = "Rumen Yankov" });

				emailService.CreateEmailMessage(model);
			}
		}
	}

}
