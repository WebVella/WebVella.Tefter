namespace WebVella.Tefter.UI.Tests.Components;
public class FilterManageComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfFilterManage>(parameters => parameters
		.Add(p => p.Item, new TucFilterAnd { })
		);

		// Assert
		cut.Find(".tf-filter-manage");

		Context.DisposeComponents();
	}

}
