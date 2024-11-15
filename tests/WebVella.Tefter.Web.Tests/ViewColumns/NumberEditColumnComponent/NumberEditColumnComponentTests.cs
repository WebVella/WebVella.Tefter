namespace WebVella.Tefter.Web.Tests.ViewColumns;

public class NumberEditColumnComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfNumberEditColumnComponent>(args => args
		.Add(x => x.Context, new TucViewColumnComponentContext())
		);

		// Assert
		cut.Find("fluent-number-field");

		Context.DisposeComponents();
	}

}
