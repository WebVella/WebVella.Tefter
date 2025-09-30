namespace WebVella.Tefter.UI.Tests.Components;

public class UserStateManagerComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfUserStateManager>();

		// Assert
		cut.Nodes.Length.Should().Be(0);

		Context.DisposeComponents();
	}

}
