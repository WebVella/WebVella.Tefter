using Fluxor;
using WebVella.Tefter.Web.Components.Login;
using WebVella.Tefter.Web.Store.UserState;

namespace WebVella.Tefter.Web.Tests.Components.Login;


public class LoginComponentTests : BaseTest
{

	[Fact]
	public void LoginComponentRendersCorrectly()
	{
		//Init
		Context.Services.AddSingleton<IState<UserState>>(UserState);
		

		// Act
		var cut = Context.RenderComponent<TfLogin>();

		// Assert
		cut.Find(".login-logo");
	}
}
