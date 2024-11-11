namespace WebVella.Tefter.Web.Tests.Components;
public class AdminDataProviderSynchronizationComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given

			// Act
			var cut = Context.RenderComponent<TfAdminDataProviderSynchronization>();

			// Assert
			cut.Find(".tf-grid-wrapper");
		}
	}
}
