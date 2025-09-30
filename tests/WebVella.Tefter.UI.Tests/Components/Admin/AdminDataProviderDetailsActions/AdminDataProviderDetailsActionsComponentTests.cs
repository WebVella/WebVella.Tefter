namespace WebVella.Tefter.UI.Tests.Components;
public class AdminDataProviderDetailsActionsComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			// Act
			var cut = Context.RenderComponent<TfAdminDataProviderDetailsActions>();

			// Assert
			if (cut.Nodes.Length == 1)
			{
				Console.WriteLine(cut.Nodes[0].NodeName);
				Console.WriteLine(cut.Nodes[0].TextContent);
				Console.WriteLine(cut.Nodes[0].NodeValue);
			}
			cut.Nodes.Length.Should().Be(0);

			Context.DisposeComponents();
		}
	}
}
