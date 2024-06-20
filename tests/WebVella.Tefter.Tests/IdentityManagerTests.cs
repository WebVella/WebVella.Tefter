namespace WebVella.Tefter.Tests;

public partial class IdentityManagerTests : BaseTest
{
	[Fact]
	public async Task CRUD_UserAndRole()
	{
		using (await locker.LockAsync())
		{
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();
			IIdentityManager identityManager = ServiceProvider.GetRequiredService<IIdentityManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var role = identityManager
					.CreateRoleBuilder()
					.WithName("UnitTester")
					.Build();

				var roleResult = await identityManager.SaveRoleAsync(role);
				roleResult.Should().NotBeNull();
				roleResult.IsSuccess.Should().BeTrue();
				roleResult.Value.Should().NotBeNull();

				role = roleResult.Value;

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

				var userResult = await identityManager.SaveUserAsync(user);
				userResult.Should().NotBeNull();
				userResult.IsSuccess.Should().BeTrue();
				userResult.Value.Should().NotBeNull();

				userResult = await identityManager.GetUserAsync("test@test.com", "password");
				userResult.Should().NotBeNull();
				userResult.IsSuccess.Should().BeTrue();
				userResult.Value.Should().NotBeNull();

				user = userResult.Value;

				user = identityManager
					.CreateUserBuilder(user)
					.WithEmail("test1@test.com")
					.WithPassword("password1")
					.WithFirstName("firstname1")
					.WithLastName("lastname1")
					.Enabled(true)
					.WithRoles(role)
					.Build();

				userResult = await identityManager.SaveUserAsync(user);
				userResult.Should().NotBeNull();
				userResult.IsSuccess.Should().BeTrue();
				userResult.Value.Should().NotBeNull();
				userResult.Value.Id.Should().Be(user.Id);
				userResult.Value.Email.Should().Be(user.Email);
				userResult.Value.FirstName.Should().Be(user.FirstName);
				userResult.Value.LastName.Should().Be(user.LastName);
				userResult.Value.Enabled.Should().Be(user.Enabled);
				userResult.Value.CreatedOn.Should().Be(user.CreatedOn);

				var deleteRoleResult = await identityManager.DeleteRoleAsync(role);
				deleteRoleResult.Should().NotBeNull();
				deleteRoleResult.IsSuccess.Should().BeFalse();

				var role2 = identityManager
					.CreateRoleBuilder()
					.WithName("UnitTester2")
					.Build();

				roleResult = await identityManager.SaveRoleAsync(role2);
				roleResult.Should().NotBeNull();
				roleResult.IsSuccess.Should().BeTrue();
				roleResult.Value.Should().NotBeNull();

				role2 = roleResult.Value;

				deleteRoleResult = await identityManager.DeleteRoleAsync(role2);
				deleteRoleResult.Should().NotBeNull();
				deleteRoleResult.IsSuccess.Should().BeTrue();
			}
		}
	}
}
