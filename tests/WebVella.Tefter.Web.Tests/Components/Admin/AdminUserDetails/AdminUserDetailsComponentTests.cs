namespace WebVella.Tefter.Web.Tests.Components;
public class AdminUserDetailsComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		var user = new TucUser { Settings = new TucUserSettings { IsSidebarOpen = true } };
		Dispatcher.Dispatch(new SetAppStateAction(
			component: null,
			state: new TfAppState { AdminManagedUser = user }));

		// Act
		var cut = Context.RenderComponent<TfAdminUserDetails>();

		// Assert
		cut.Find(".tf-card");

		Context.DisposeComponents();
	}
}
