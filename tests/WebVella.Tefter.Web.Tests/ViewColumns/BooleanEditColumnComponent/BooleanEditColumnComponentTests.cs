namespace WebVella.Tefter.Web.Tests.ViewColumns;

public class BooleanEditColumnComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfBooleanEditColumnComponent>(args => args
		.Add(x => x.Context, new TucViewColumnComponentContext())
		);

		// Assert
		cut.Find("fluent-checkbox");

		Context.DisposeComponents();
	}

}
