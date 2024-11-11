namespace WebVella.Tefter.Web.Tests.Components;
public class FilterCardComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given

			// Act
			var cut = Context.RenderComponent<TfFilterCard>();

			// Assert
			cut.Find(".tf-card");
		}
	}

}
