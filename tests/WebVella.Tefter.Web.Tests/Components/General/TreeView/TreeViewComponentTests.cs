namespace WebVella.Tefter.Web.Tests.Components;
public class TreeViewComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		var menu = new List<TucMenuItem>{
				new TucMenuItem{ Text = "test"}
			};
		// Act
		var cut = Context.RenderComponent<TfTreeView>(p => p
		.Add(x => x.Items, menu));

		// Assert
		cut.Find(".tf-menu");

		Context.DisposeComponents();
	}

}
