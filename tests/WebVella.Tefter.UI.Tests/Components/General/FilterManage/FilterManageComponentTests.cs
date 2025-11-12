namespace WebVella.Tefter.UI.Tests.Components;
public class FilterManageComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.Render<TucFilterManage>(parameters => parameters
		.Add(p => p.Item, new TfFilterAnd { })
		);

		// Assert
		cut.Find(".tf-filter-manage");

		Context.Dispose();
	}

}
