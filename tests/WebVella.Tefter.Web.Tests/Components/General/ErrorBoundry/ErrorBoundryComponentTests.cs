namespace WebVella.Tefter.Web.Tests.Components;
public class ErrorBoundryComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
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

}
