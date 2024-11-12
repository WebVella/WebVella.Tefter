namespace WebVella.Tefter.Web.Tests.Components;
public class LoginComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
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
	}

	[Fact]
	public async Task LoginComponentLocalizationCorrect()
	{
		using (await locker.LockAsync())
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
}
