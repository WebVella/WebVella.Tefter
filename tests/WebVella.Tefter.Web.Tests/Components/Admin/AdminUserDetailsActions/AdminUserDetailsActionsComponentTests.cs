namespace WebVella.Tefter.Web.Tests.Components;
public class AdminUserDetailsActionsComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		var route = new TucRouteState();
		route = route with { ThirdNode = RouteDataThirdNode.Details };
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
