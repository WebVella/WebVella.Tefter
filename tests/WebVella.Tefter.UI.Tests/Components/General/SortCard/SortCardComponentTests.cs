namespace WebVella.Tefter.UI.Tests.Components;
public class SortCardComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfSortCard>();

		// Assert
		cut.Find(".tf-card");

		Context.DisposeComponents();
	}

}
