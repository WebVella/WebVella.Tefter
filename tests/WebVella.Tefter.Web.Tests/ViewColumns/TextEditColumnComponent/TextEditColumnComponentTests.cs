namespace WebVella.Tefter.Web.Tests.ViewColumns;

public class TextEditColumnComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfTextEditColumnComponent>(args => args
		.Add(x => x.Context, new TucViewColumnComponentContext())
		);

		// Assert
		cut.Find("fluent-text-field");

		Context.DisposeComponents();
	}

}
