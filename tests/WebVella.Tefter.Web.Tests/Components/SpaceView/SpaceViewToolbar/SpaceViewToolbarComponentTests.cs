namespace WebVella.Tefter.Web.Tests.Components;

public class SpaceViewToolbarComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		Dispatcher.Dispatch(new SetAppStateAction(
		component: null,
		state: new TfAppState { 
			Route = new TucRouteState(),
			SpaceView = new(),
			SelectedDataRows = new()
			}));
		// Act
		var cut = Context.RenderComponent<TfSpaceViewToolbar>();

		// Assert
		cut.Find(".content-toolbar");

		Context.DisposeComponents();
	}

}
