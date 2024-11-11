namespace WebVella.Tefter.Web.Tests.Components;
public class UserNavigationComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var user = new TucUser{ FirstName = "First", LastName = "Last" };
			UserStateUseCaseMock.Setup(x=> x.GetUserFromCookieAsync()).Returns(Task.FromResult(user));
			// Act
			var cut = Context.RenderComponent<TfUserNavigation>();

			// Assert
			cut.Find(".usernav");
		}
	}

}
