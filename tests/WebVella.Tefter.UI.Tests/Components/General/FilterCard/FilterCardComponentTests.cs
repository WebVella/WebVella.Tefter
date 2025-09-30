namespace WebVella.Tefter.UI.Tests.Components;
public class FilterCardComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfFilterCard>();

		// Assert
		cut.Find(".tf-card");

		Context.DisposeComponents();
	}

}
