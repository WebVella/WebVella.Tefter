namespace WebVella.Tefter.Tests.Applications;

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
			ITfService tfService = ServiceProvider.GetService<ITfService>();


			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				Guid relatedRowId = Guid.NewGuid();

				var user = tfService.GetUser("rumen@webvella.com");
				CreateEmailMessageModel model = new CreateEmailMessageModel();
				model.Subject = "test";
				model.TextBody = "";
				model.HtmlBody = "<html><body><p>test html</p><img src='/fs/repository/ai_avatar.jpg'>people link</img></body></html>";
				model.Recipients.Add(new EmailAddress { Address = "rumen.yankov@gmail.com", Name = "Rumen Yankov" });
				model.UserId = user.Id;
				model.RelatedRowIds.Add(relatedRowId);

				emailService.CreateEmailMessage(model);
				var emailsList = emailService.GetEmailMessages();

				foreach (var email in emailsList)
				{
					var emailById = emailService.GetEmailMessageById(email.Id);
					emailById.Should().NotBeNull();
				}

				var searchResult = emailService.GetEmailMessages("rumen");

				emailsList = emailService.GetEmailMessages(relatedRowId);
				emailsList.Count.Should().Be(1);
			}
		}
	}
}
