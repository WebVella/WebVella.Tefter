namespace WebVella.Tefter.Web.Tests.Components;
public class SortCardComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given

			// Act
			var cut = Context.RenderComponent<TfSortCard>();

			// Assert
			cut.Find(".tf-card");
		}
	}

}
