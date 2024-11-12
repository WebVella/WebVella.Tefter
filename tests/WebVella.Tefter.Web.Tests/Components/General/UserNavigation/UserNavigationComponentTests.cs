namespace WebVella.Tefter.Web.Tests.Components;
public class UserNavigationComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			var user = new TucUser{ FirstName = "First", LastName = "Last" };
			Dispatcher.Dispatch(new SetUserStateAction(
			component: null,
			state: new TfUserState { CurrentUser = user }));
			// Act
			var cut = Context.RenderComponent<TfUserNavigation>();

			// Assert
			cut.Find(".usernav");

			Context.DisposeComponents();
		}
	}

}
