namespace WebVella.Tefter.Web.Tests.Components;
public class AdminDataProviderDetailsComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given

			// Act
			var cut = Context.RenderComponent<TfAdminDataProviderDetails>();

			// Assert
			cut.Find(".tf-card");
		}
	}
}
