namespace WebVella.Tefter.Tests;

using WebVella.Tefter.EmailSender.Models;
using WebVella.Tefter.EmailSender.Services;


public partial class TalkTests : BaseTest
{
	[Fact]
	public async Task Email_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			IEmailService talkService = ServiceProvider.GetRequiredService<IEmailService>();
			IIdentityManager identityManager = ServiceProvider.GetRequiredService<IIdentityManager>();
			ITfDataManager dataManager = ServiceProvider.GetRequiredService<ITfDataManager>();


			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var user = identityManager.GetUser("rumen@webvella.com").Value;

			}
		}
	}

}
