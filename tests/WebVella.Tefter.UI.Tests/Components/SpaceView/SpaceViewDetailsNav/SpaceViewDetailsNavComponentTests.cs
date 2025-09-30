namespace WebVella.Tefter.UI.Tests.Components;

public class SpaceViewDetailsNavComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		Dispatcher.Dispatch(new SetAppStateAction(
		component: null,
		state: new TfAppState { Route = new TucRouteState() }));
		// Act
		var cut = Context.RenderComponent<TfSpaceViewDetailsNav>();

		// Assert
		cut.Find(".tf-tabnav");

		Context.DisposeComponents();
	}

}
