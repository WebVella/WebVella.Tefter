namespace WebVella.Tefter.Web.Tests.Components;
public class ErrorBoundryComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfErrorBoundry>();

		// Assert
		cut.Nodes.Length.Should().Be(0);

		Context.DisposeComponents();
	}

}
