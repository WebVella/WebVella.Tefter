namespace WebVella.Tefter.Web.Tests.Components;

public class EditorComponentTests : BaseTest
{

	[Fact]
	public async Task SidebarToggleComponentRendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given

			// Act
			var cut = Context.RenderComponent<TfEditor>();

			// Assert
			cut.Find(".tf-editor");
		}
	}

}
