namespace WebVella.Tefter.UI.Tests.Components;
public class UserNavigationComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		var user = new TucUser { FirstName = "First", LastName = "Last" };
		Dispatcher.Dispatch(new SetUserStateAction(
		component: null,
		state: new TfUserState { Hash = Guid.NewGuid(), CurrentUser = user }));
		// Act
		var cut = Context.RenderComponent<TfUserNavigation>();

		// Assert
		cut.Find(".usernav");

		Context.DisposeComponents();
	}

}
