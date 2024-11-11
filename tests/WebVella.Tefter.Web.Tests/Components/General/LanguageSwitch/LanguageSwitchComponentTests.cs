namespace WebVella.Tefter.Web.Tests.Components;
public class LanguageSwitchComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given

			// Act
			var cut = Context.RenderComponent<TfLanguageSwitch>();

			// Assert
			cut.Find(".language-switch");
		}
	}
}
