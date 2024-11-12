namespace WebVella.Tefter.Web.Tests.Components;
public class ColumnCardComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		var items = new List<string> { "col1", "col2" };
		// Act
		var cut = Context.RenderComponent<TfColumnCard>(parameters => parameters
		.Add(p => p.Items, items)
		);

		// Assert
		var mainWrapper = cut.Find($".tf-card");

		Context.DisposeComponents();
	}

}
