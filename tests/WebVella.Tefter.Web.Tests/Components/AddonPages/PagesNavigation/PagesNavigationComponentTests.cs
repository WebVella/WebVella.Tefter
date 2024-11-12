namespace WebVella.Tefter.Web.Tests.Components;
public class PagesNavigationComponentTests : BaseTest
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
			state: new TfUserState { CurrentUser = user }));
			// Act
			var cut = Context.RenderComponent<TfPagesNavigation>();

			// Assert
			cut.Find(".tf-layout__body__aside");

			Context.DisposeComponents();
		}
	}
}
