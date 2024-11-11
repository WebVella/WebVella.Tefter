using Microsoft.FluentUI.AspNetCore.Components;
using System.Globalization;
using WebVella.Tefter.Web.Components;

namespace WebVella.Tefter.Web.Tests.Components;


public class LoginComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given

			// Act
			var cut = Context.RenderComponent<TfLogin>();

			// Assert
			cut.Find(".login-logo");
			cut.Find(".fluent-input-label[for='email']").TextContent.Equals("Email");
		}
	}

	[Fact]
	public async Task LoginComponentLocalizationCorrect()
	{
		using (await locker.LockAsync())
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
}
