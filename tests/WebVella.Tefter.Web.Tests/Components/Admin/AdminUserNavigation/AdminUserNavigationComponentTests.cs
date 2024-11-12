namespace WebVella.Tefter.Web.Tests.Components;
public class AdminUserNavigationComponentTests : BaseTest
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
		var cut = Context.RenderComponent<TfAdminUserNavigation>();

		// Assert
		cut.Find(".tf-layout__body__aside");

		Context.DisposeComponents();
	}
}
