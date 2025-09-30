namespace WebVella.Tefter.UI.Tests.Components;
public class AdminDataProviderNavigationComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			var user = new TucUser { Settings = new TucUserSettings { IsSidebarOpen = true } };
			Dispatcher.Dispatch(new SetUserStateAction(
			component: null,
			state: new TfUserState { Hash = Guid.NewGuid(), CurrentUser = user }));
			// Act
			var cut = Context.RenderComponent<TfAdminDataProviderNavigation>();

			// Assert
			cut.Find(".tf-layout__body__aside");

			Context.DisposeComponents();
		}
	}
}
