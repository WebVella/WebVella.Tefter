namespace WebVella.Tefter.UI.Tests.Components;
public class AdminDataProviderDataComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			// Act
			var cut = Context.RenderComponent<TfAdminDataProviderData>();

			// Assert
			cut.Find(".tf-layout__body__main");

			Context.DisposeComponents();
		}
	}
}
