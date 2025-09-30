namespace WebVella.Tefter.UI.Tests.Components;
public class AdminUserDetailsNavComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			// Act
			var cut = Context.RenderComponent<TfAdminUserDetailsNav>();

			// Assert
			cut.Find(".tf-tabnav");

			Context.DisposeComponents();
		}
	}
}
