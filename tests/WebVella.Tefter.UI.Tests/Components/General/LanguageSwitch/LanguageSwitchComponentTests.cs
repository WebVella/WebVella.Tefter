using WebVella.Tefter.UI.Layout;

namespace WebVella.Tefter.UI.Tests.Components;
public class LanguageSwitchComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TucLanguageSwitch>(c => 
				c.Add(p => p.User, new TfUser())			
			);

		// Assert
		cut.Find("#language-switch-btn");

		Context.DisposeComponents();
	}
}
