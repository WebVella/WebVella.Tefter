namespace WebVella.Tefter.UI.Tests.Components;

public class SpaceDataManageComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		Dispatcher.Dispatch(new SetAppStateAction(
		component: null,
		state: new TfAppState
		{
			Route = new TucRouteState(),
			SpaceData = new TucSpaceData(),
			CurrentUser = new TucUser()
		}));
		// Act
		var cut = Context.RenderComponent<TfSpaceDataManage>();

		// Assert
		cut.Find(".tf-card");

		Context.DisposeComponents();
	}

}
