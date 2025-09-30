namespace WebVella.Tefter.UI.Tests.Components;
public class ColumnCardComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			var items = new List<string> { "col1", "col2" };
			// Act
			var cut = Context.RenderComponent<TucColumnCard>(parameters => parameters
			.Add(p => p.Items, items)
			);

			// Assert
			var mainWrapper = cut.Find($".tf-card");

			Context.DisposeComponents();
		}
	}

}
