using WebVella.Tefter.Web.Store;

namespace WebVella.Tefter.Web.Tests.Components;
public class AdminUserDetailsComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var user = new TucUser { Settings = new TucUserSettings { IsSidebarOpen = true } };
			var tfUserState = new TfUserState { CurrentUser = user };
			UserStateUseCaseMock.Setup(x => x.InitUserState()).Returns(Task.FromResult(tfUserState));
			UserStateUseCaseMock.Setup(x => x.GetUserFromCookieAsync()).Returns(Task.FromResult(user));
			AppStateUseCaseMock.Setup(x => x.GetUserAsync(It.IsAny<Guid>())).Returns(Task.FromResult(user));
			Context.RenderComponent<TfUserStateManager>();
			Context.RenderComponent<TfAppStateManager>();
			// Act
			var cut = Context.RenderComponent<TfAdminUserDetails>();

			// Assert
			cut.Find(".tf-card");
		}
	}
}
