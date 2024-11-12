namespace WebVella.Tefter.Web.Tests.Components;
public class PagesContentComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			// Act
			var cut = Context.RenderComponent<TfPagesContent>();

			// Assert
			cut.Find(".tf-layout__body__main");

			Context.DisposeComponents();
		}
	}
}
