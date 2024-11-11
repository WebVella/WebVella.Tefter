namespace WebVella.Tefter.Web.Tests.Components;
public class TreeViewItemComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var menu = new TucMenuItem{ Text = "test"};
			// Act
			var cut = Context.RenderComponent<TfTreeViewItem>(p=>p
			.Add(x=> x.Item,menu));

			// Assert
			cut.Find(".tf-menu-item");
		}
	}

}
