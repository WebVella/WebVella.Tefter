namespace WebVella.Tefter.UI.Tests.Components;
public class AdminFileRepositoryComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			// Act
			var cut = Context.RenderComponent<TfAdminFileRepository>();

			// Assert
			cut.Find(".tf-admin-repository-toolbar");

			Context.DisposeComponents();
		}
	}
}
