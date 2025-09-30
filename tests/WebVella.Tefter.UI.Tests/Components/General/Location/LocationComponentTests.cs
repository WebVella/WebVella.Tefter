namespace WebVella.Tefter.UI.Tests.Components;
public class LocationComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfLocation>();

		// Assert
		cut.Find(".location");

		Context.DisposeComponents();
	}
}
