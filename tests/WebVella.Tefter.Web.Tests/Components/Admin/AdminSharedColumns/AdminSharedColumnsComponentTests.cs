namespace WebVella.Tefter.Web.Tests.Components;
public class AdminSharedColumnsComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			// Act
			var cut = Context.RenderComponent<TfAdminSharedColumns>();

			// Assert
			cut.Find(".tf-admin-shared-columns-toolbar");

			Context.DisposeComponents();
		}
	}
}
