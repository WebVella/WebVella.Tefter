namespace WebVella.Tefter.Web.Tests.Components;
public class AdminFileRepositoryComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given

			// Act
			var cut = Context.RenderComponent<TfAdminFileRepository>();

			// Assert
			cut.Find(".tf-admin-repository-toolbar");
		}
	}
}
