namespace WebVella.Tefter.Web.Tests.Components;
public class FilterManageComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			// Act
			var cut = Context.RenderComponent<TfFilterManage>(parameters => parameters
			.Add(p => p.Item, new TucFilterAnd{ })
			);

			// Assert
			cut.Find(".tf-filter-manage");

			Context.DisposeComponents();
		}
	}

}
