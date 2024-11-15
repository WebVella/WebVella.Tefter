namespace WebVella.Tefter.Web.Tests.Components;

public class SpaceViewHeaderComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		Dispatcher.Dispatch(new SetAppStateAction(
		component: null,
		state: new TfAppState { SpaceView = new TucSpaceView() }));
		// Act
		var cut = Context.RenderComponent<TfSpaceViewHeader>();

		// Assert
		cut.Find(".content-header");

		Context.DisposeComponents();
	}

}
