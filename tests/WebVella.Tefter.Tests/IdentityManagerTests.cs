using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.Tests;

public partial class IdentityManagerTests : BaseTest
{
	[Fact]
	public async Task CRUD_UserAndRole()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			IIdentityManager identityManager = ServiceProvider.GetRequiredService<IIdentityManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var role = identityManager
					.CreateRoleBuilder()
					.WithName("UnitTester")
					.Build();

				role = await identityManager.SaveRoleAsync(role);
				role.Should().NotBeNull();

				var user = identityManager
					.CreateUserBuilder()
					.WithEmail("test@test.com")
					.WithPassword("password")
					.WithFirstName("firstname")
					.WithLastName("lastname")
					.CreatedOn(DateTime.Now)
					.Enabled(true)
					.WithRoles(role)
					.Build();

				user = await identityManager.SaveUserAsync(user);
				user.Should().NotBeNull();

				user = await identityManager.GetUserAsync("test@test.com", "password");
				user.Should().NotBeNull();

				user = identityManager
					.CreateUserBuilder(user)
					.WithEmail("test1@test.com")
					.WithPassword("password1")
					.WithFirstName("firstname1")
					.WithLastName("lastname1")
					.Enabled(true)
					.WithRoles(role)
					.Build();

				user = await identityManager.SaveUserAsync(user);
				user.Should().NotBeNull();
				user.Id.Should().Be(user.Id);
				user.Email.Should().Be(user.Email);
				user.FirstName.Should().Be(user.FirstName);
				user.LastName.Should().Be(user.LastName);
				user.Enabled.Should().Be(user.Enabled);
				user.CreatedOn.Should().Be(user.CreatedOn);


				var task = Task.Run(async () => { await identityManager.DeleteRoleAsync(role); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);

				var role2 = identityManager
					.CreateRoleBuilder()
					.WithName("UnitTester2")
					.Build();

				role2 = await identityManager.SaveRoleAsync(role2);
				role2.Should().NotBeNull();

				await identityManager.DeleteRoleAsync(role2);
			}
		}
	}

	[Fact]
	public async Task User_GetWithNoEmailAndPassword()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			IIdentityManager identityManager = ServiceProvider.GetRequiredService<IIdentityManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{

				var task = Task.Run(() => { var user = identityManager.GetUser(null, null); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(2);
				((TfValidationException)exception).Data.Contains(nameof(User.Email)).Should().BeTrue();
				((TfValidationException)exception).Data.Contains(nameof(User.Password)).Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task User_RemoveAllRoles()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			IIdentityManager identityManager = ServiceProvider.GetRequiredService<IIdentityManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var role = identityManager
					.CreateRoleBuilder()
					.WithName("UnitTester")
					.Build();

				role = await identityManager.SaveRoleAsync(role);
				role.Should().NotBeNull();

				var user = identityManager
					.CreateUserBuilder()
					.WithEmail("test@test.com")
					.WithPassword("password")
					.WithFirstName("firstname")
					.WithLastName("lastname")
					.CreatedOn(DateTime.Now)
					.Enabled(true)
					.WithRoles(role)
					.Build();

				user = await identityManager.SaveUserAsync(user);
				user.Should().NotBeNull();

				user = identityManager
					.CreateUserBuilder(user)
					.WithEmail("test1@test.com")
					.WithPassword("password1")
					.WithFirstName("firstname1")
					.WithLastName("lastname1")
					.Enabled(true)
					.WithRoles()
					.Build();

				user = await identityManager.SaveUserAsync(user);
				user.Should().NotBeNull();
			}
		}
	}
}
