namespace WebVella.Tefter.Web.Tests.Components;

public class EditorComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfEditor>();

		// Assert
		cut.Find(".tf-editor");

		Context.DisposeComponents();
	}

}
