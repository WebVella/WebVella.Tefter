namespace WebVella.Tefter.UI.Tests.Components;
public class SidebarToggleComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
			//Given
			var Context = GetTestContext();
			// Act
			var cut = Context.RenderComponent<TfSidebarToggle>();

			// Assert
			cut.Find("fluent-button");

			Context.DisposeComponents();
	}

}
