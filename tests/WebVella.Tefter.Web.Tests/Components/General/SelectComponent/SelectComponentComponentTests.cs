namespace WebVella.Tefter.Web.Tests.Components;
public class SelectComponentComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfSelectComponent<string>>();

		// Assert
		cut.Find(".tf-select-component");

		Context.DisposeComponents();
	}
}
