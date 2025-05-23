﻿using WebVella.Tefter.Models;

namespace WebVella.Tefter.Tests.Services;

public partial class TfServiceTest : BaseTest
{
	[Fact]
	public async Task User_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var role = tfService
					.CreateRoleBuilder()
					.WithName("UnitTester")
					.Build();

				role = await tfService.SaveRoleAsync(role);
				role.Should().NotBeNull();

				var user = tfService
					.CreateUserBuilder()
					.WithEmail("test@test.com")
					.WithPassword("password")
					.WithFirstName("firstname")
					.WithLastName("lastname")
					.CreatedOn(DateTime.Now)
					.Enabled(true)
					.WithRoles(role)
					.Build();

				user = await tfService.SaveUserAsync(user);
				user.Should().NotBeNull();

				user = await tfService.GetUserAsync("test@test.com", "password");
				user.Should().NotBeNull();

				user = tfService
					.CreateUserBuilder(user)
					.WithEmail("test1@test.com")
					.WithPassword("password1")
					.WithFirstName("firstname1")
					.WithLastName("lastname1")
					.Enabled(true)
					.WithRoles(role)
					.Build();

				user = await tfService.SaveUserAsync(user);
				user.Should().NotBeNull();
				user.Id.Should().Be(user.Id);
				user.Email.Should().Be(user.Email);
				user.FirstName.Should().Be(user.FirstName);
				user.LastName.Should().Be(user.LastName);
				user.Enabled.Should().Be(user.Enabled);
				user.CreatedOn.Should().Be(user.CreatedOn);


				var task = Task.Run(async () => { await tfService.DeleteRoleAsync(role); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);

				var role2 = tfService
					.CreateRoleBuilder()
					.WithName("UnitTester2")
					.Build();

				role2 = await tfService.SaveRoleAsync(role2);
				role2.Should().NotBeNull();

				await tfService.DeleteRoleAsync(role2);
			}
		}
	}

	[Fact]
	public async Task User_AddAndRemoveToRole()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var role = tfService
					.CreateRoleBuilder()
					.WithName("UnitTester")
					.Build();

				role = await tfService.SaveRoleAsync(role);
				role.Should().NotBeNull();

				var user1 = tfService
					.CreateUserBuilder()
					.WithEmail("test1@test.com")
					.WithPassword("password")
					.WithFirstName("firstname")
					.WithLastName("lastname")
					.CreatedOn(DateTime.Now)
					.Enabled(true)
					.Build();

				user1 = await tfService.SaveUserAsync(user1);
				user1.Should().NotBeNull();
				user1.Roles.Count().Should().Be(0);

				var user2 = tfService
				.CreateUserBuilder()
				.WithEmail("test2@test.com")
				.WithPassword("password")
				.WithFirstName("firstname")
				.WithLastName("lastname")
				.CreatedOn(DateTime.Now)
				.Enabled(true)
				.Build();

				user2 = await tfService.SaveUserAsync(user2);
				user2.Should().NotBeNull();
				user2.Roles.Count().Should().Be(0);

				await tfService.AddUsersRoleAsync( new List<TfUser> { user1, user2 }, role);

				user1 = await tfService.GetUserAsync(user1.Id);
				user1.Roles.Count().Should().Be(1);

				user2 = await tfService.GetUserAsync(user2.Id);
				user2.Roles.Count().Should().Be(1);

				await tfService.RemoveUsersRoleAsync(new List<TfUser> { user1, user2 }, role);

				user1 = await tfService.GetUserAsync(user1.Id);
				user1.Roles.Count().Should().Be(0);

				user2 = await tfService.GetUserAsync(user2.Id);
				user2.Roles.Count().Should().Be(0);
			}
		}
	}

	[Fact]
	public async Task User_GetWithNoEmailAndPassword()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{

				var task = Task.Run(() => { var user = tfService.GetUser(null, null); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(2);
				((TfValidationException)exception).Data.Contains(nameof(TfUser.Email)).Should().BeTrue();
				((TfValidationException)exception).Data.Contains(nameof(TfUser.Password)).Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task User_RemoveAllRoles()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var role = tfService
					.CreateRoleBuilder()
					.WithName("UnitTester")
					.Build();

				role = await tfService.SaveRoleAsync(role);
				role.Should().NotBeNull();

				var user = tfService
					.CreateUserBuilder()
					.WithEmail("test@test.com")
					.WithPassword("password")
					.WithFirstName("firstname")
					.WithLastName("lastname")
					.CreatedOn(DateTime.Now)
					.Enabled(true)
					.WithRoles(role)
					.Build();

				user = await tfService.SaveUserAsync(user);
				user.Should().NotBeNull();

				user = tfService
					.CreateUserBuilder(user)
					.WithEmail("test1@test.com")
					.WithPassword("password1")
					.WithFirstName("firstname1")
					.WithLastName("lastname1")
					.Enabled(true)
					.WithRoles()
					.Build();

				user = await tfService.SaveUserAsync(user);
				user.Should().NotBeNull();
			}
		}
	}
}
