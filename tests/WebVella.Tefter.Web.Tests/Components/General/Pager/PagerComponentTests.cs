namespace WebVella.Tefter.Web.Tests.Components;
public class PagerComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			// Act
			var cut = Context.RenderComponent<TfPager>();

			// Assert
			cut.Find(".paginator-nav");

			Context.DisposeComponents();
		}
	}
}
