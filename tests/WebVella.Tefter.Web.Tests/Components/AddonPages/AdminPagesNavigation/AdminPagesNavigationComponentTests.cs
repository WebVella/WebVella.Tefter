namespace WebVella.Tefter.Web.Tests.Components;
public class AdminPagesNavigationComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var user = new TucUser { Settings = new TucUserSettings { IsSidebarOpen = true } };
			UserStateUseCaseMock.Setup(x => x.GetUserFromCookieAsync()).Returns(Task.FromResult(user));
			// Act
			var cut = Context.RenderComponent<TfAdminPagesNavigation>();

			// Assert
			cut.Find(".tf-layout__body__aside");
		}
	}
}
