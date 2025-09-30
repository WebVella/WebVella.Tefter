namespace WebVella.Tefter.UI.Tests.Components;

public class SpaceManageNodeItemComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfSpaceManageNodeItem>(parameters => parameters
		.Add(p => p.Item, new TucSpaceNode()));

		// Assert
		cut.Find(".tf-grid-tr");

		Context.DisposeComponents();
	}

}
