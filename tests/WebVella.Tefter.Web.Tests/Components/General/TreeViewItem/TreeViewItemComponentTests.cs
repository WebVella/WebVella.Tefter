namespace WebVella.Tefter.Web.Tests.Components;
public class TreeViewItemComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		var menu = new TucMenuItem { Text = "test" };
		// Act
		var cut = Context.RenderComponent<TfTreeViewItem>(p => p
		.Add(x => x.Item, menu));

		// Assert
		cut.Find(".tf-menu__item");

		Context.DisposeComponents();
	}

}
