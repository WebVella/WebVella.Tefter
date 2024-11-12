namespace WebVella.Tefter.Web.Tests.Components;
public class PageHeaderComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			// Act
			var cut = Context.RenderComponent<TfPageHeader>();

			// Assert
			cut.Find(".page-header");

			Context.DisposeComponents();
		}
	}
}
