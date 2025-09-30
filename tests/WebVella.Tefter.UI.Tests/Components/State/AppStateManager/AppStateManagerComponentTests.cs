namespace WebVella.Tefter.UI.Tests.Components;

public class AppStateManagerComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfAppStateManager>();

		// Assert
		cut.Nodes.Length.Should().Be(0);

		Context.DisposeComponents();
	}

}
