namespace WebVella.Tefter.Web.Tests.Components;
public class AdminDataProviderDetailsNavComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given

			// Act
			var cut = Context.RenderComponent<TfAdminDataProviderDetailsNav>();

			// Assert
			cut.Find(".tf-tabnav");
		}
	}
}
