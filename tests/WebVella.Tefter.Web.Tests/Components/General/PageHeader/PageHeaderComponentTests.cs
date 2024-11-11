namespace WebVella.Tefter.Web.Tests.Components;
public class PageHeaderComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given

			// Act
			var cut = Context.RenderComponent<TfPageHeader>();

			// Assert
			cut.Find(".page-header");
		}
	}
}
