//using FluentResults;

//namespace WebVella.Tefter.Migrations;

//[TefterSystemMigration("2024.6.21.1")]
//internal class TefterSystemMigration2024062101 : TefterSystemMigration
//{
//	public override async Task MigrateDataAsync(IServiceProvider serviceProvider)
//	{
//		//adds users for developers
//		//this migration will be removed for production or at later stage

//#if DEBUG

//		IDatabaseService dbService = serviceProvider.GetService<IDatabaseService>();
//		IDboManager dboManager = serviceProvider.GetService<IDboManager>();
//		IIdentityManager identityManager = serviceProvider.GetService<IIdentityManager>();

//		var adminRoleResult = identityManager.GetRole("Administrators");
//		if (!adminRoleResult.IsSuccess)
//			throw new DatabaseException("Failed to get admin role.");

//		var adminRole = adminRoleResult.Value;

//		var user = identityManager
//			.CreateUserBuilder()
//			.WithEmail("rumen@webvella.com")
//			.WithFirstName("Rumen")
//			.WithLastName("Yankov")
//			.CreatedOn(DateTime.Now)
//			.WithPassword("123")
//			.Enabled(true)
//			.WithRoles(adminRole)
//			.Build();

//		var userResult = await identityManager.SaveUserAsync(user);
//		if (!userResult.IsSuccess)
//			throw new DatabaseException("Failed to create Rumen Yankov user");

//		user = identityManager
//		   .CreateUserBuilder()
//		   .WithEmail("boz@webvella.com")
//		   .WithFirstName("Bozhidar")
//		   .WithLastName("Zashev")
//		   .CreatedOn(DateTime.Now)
//		   .WithPassword("123")
//		   .Enabled(true)
//		   .WithRoles(adminRole)
//		   .Build();

//		userResult = await identityManager.SaveUserAsync(user);
//		if (!userResult.IsSuccess)
//			throw new DatabaseException("Failed to create Bozhidar Zashev user");

//#endif
//	}


//}
