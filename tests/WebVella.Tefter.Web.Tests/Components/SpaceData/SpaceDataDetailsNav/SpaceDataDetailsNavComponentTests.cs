namespace WebVella.Tefter.Web.Tests.Components;

public class SpaceDataDetailsNavComponentTests : BaseTest
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
		var cut = Context.RenderComponent<TfSpaceDataDetailsNav>();

		// Assert
		cut.Find(".tf-tabnav");

		Context.DisposeComponents();
	}

}
