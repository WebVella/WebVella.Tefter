namespace WebVella.Tefter.UI.Tests.Components;
public class AdminDataProviderDetailsNavComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			// Act
			var cut = Context.RenderComponent<TfAdminDataProviderDetailsNav>();

			// Assert
			cut.Find(".tf-tabnav");

			Context.DisposeComponents();
		}
	}
}
