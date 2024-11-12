namespace WebVella.Tefter.Web.Tests.Components;
public class LoginComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfLogin>();

		// Assert
		cut.Find(".login-logo");
		cut.Find(".fluent-input-label[for='email']").TextContent.Equals("Email");

		Context.DisposeComponents();
	}

	[Fact]
	public void LoginComponentLocalizationCorrect()
	{
		//Given
		var Context = GetTestContext();
		CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("bg-BG");
		CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("bg-BG");

		// Act
		var cut = Context.RenderComponent<TfLogin>();

		// Assert
		cut.Find(".login-logo");
		cut.Find(".fluent-input-label[for='email']").TextContent.Equals("Имейл");

		Context.DisposeComponents();
	}
}
