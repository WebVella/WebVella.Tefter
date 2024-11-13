namespace WebVella.Tefter.Tests;

using WebVella.Tefter.EmailSender.Models;
using WebVella.Tefter.EmailSender.Services;


public partial class EmailSenderTests : BaseTest
{
	[Fact]
	public async Task EmailSender_CreateAndGet()
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
				model.UserId = user.Id;

				var result = emailService.CreateEmailMessage(model);
				result.IsSuccess.Should().BeTrue();

				var emailsListResult = emailService.GetEmailMessages();
				emailsListResult.IsSuccess.Should().BeTrue();

				foreach (var email in emailsListResult.Value)
				{
					var emailByIdResult = emailService.GetEmailMessageById(email.Id);
					emailByIdResult.IsSuccess.Should().BeTrue();
					emailByIdResult.Value.Should().NotBeNull();

				}
			}
		}
	}

}
