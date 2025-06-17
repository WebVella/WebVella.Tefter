namespace WebVella.Tefter.Web.Tests.Components;
public class AdminUserDetailsComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			var user = new TucUser { Settings = new TucUserSettings { IsSidebarOpen = true } };
			Dispatcher.Dispatch(new SetAppStateAction(
				component: null,
				state: new TfAppState { AdminManagedUser = user, Route = new TucRouteState() }));

			// Act
			var cut = Context.RenderComponent<TfAdminUserDetails>();

			// Assert
			cut.Find(".tf-card");

			Context.DisposeComponents();
		}
	}
}
