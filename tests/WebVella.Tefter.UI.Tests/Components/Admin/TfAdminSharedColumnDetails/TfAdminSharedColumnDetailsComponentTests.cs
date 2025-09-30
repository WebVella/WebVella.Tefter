namespace WebVella.Tefter.UI.Tests.Components;
public class TfAdminSharedColumnDetailsComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			// Act
			var cut = Context.RenderComponent<TfAdminSharedColumnDetails>();

			// Assert
			cut.Find(".tf-card");

			Context.DisposeComponents();
		}
	}
}
