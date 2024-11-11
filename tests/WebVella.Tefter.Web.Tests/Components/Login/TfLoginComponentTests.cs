using Microsoft.FluentUI.AspNetCore.Components;
using System.Globalization;
using WebVella.Tefter.Web.Components;

namespace WebVella.Tefter.Web.Tests.Components.Login;


public class LoginComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given

		// Act
		var cut = Context.RenderComponent<TfLogin>();

		// Assert
		cut.Find(".login-logo");
		cut.Find(".fluent-input-label[for='email']").TextContent.Equals("Email");
	}

	[Fact]
	public void LoginComponentLocalizationCorrect()
	{
		//Given
		CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("bg-BG");
		CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("bg-BG");

		// Act
		var cut = Context.RenderComponent<TfLogin>();

		// Assert
		cut.Find(".login-logo");
		cut.Find(".fluent-input-label[for='email']").TextContent.Equals("Имейл");
	}
}
