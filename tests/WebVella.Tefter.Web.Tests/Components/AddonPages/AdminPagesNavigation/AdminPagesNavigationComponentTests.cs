namespace WebVella.Tefter.Web.Tests.Components;
public class AdminPagesNavigationComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();

		var user = new TucUser { Settings = new TucUserSettings { IsSidebarOpen = true } };
		Dispatcher.Dispatch(new SetUserStateAction(
		component: null,
		state: new TfUserState { CurrentUser = user }));
		// Act
		var cut = Context.RenderComponent<TfAdminPagesNavigation>();

		// Assert
		cut.Find(".tf-layout__body__aside");

		Context.DisposeComponents();
	}
}
