namespace WebVella.Tefter.UI.Tests.Components;
public class PagesContentComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfPagesContent>();

		// Assert
		cut.Find(".tf-layout__body__main");

		Context.DisposeComponents();
	}
}
