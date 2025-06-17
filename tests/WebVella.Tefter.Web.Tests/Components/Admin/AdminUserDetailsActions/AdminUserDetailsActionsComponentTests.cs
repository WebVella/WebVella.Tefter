namespace WebVella.Tefter.Web.Tests.Components;
public class AdminUserDetailsActionsComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			var route = new TucRouteState();
			route = route with { RouteNodes = new List<RouteDataNode> { RouteDataNode.Admin, RouteDataNode.Users, RouteDataNode.UserId } };
			Dispatcher.Dispatch(new SetAppStateAction(
				component: null,
				state: new TfAppState { Route = route }));

			// Act
			var cut = Context.RenderComponent<TfAdminUserDetailsActions>();

			// Assert
			cut.Find("fluent-button");

			Context.DisposeComponents();
		}
	}
}
