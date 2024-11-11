namespace WebVella.Tefter.Web.Tests.Components;
public class AdminDataProviderKeysComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given

			// Act
			var cut = Context.RenderComponent<TfAdminDataProviderKeys>();

			// Assert
			cut.Find(".fluent-messagebar");
		}
	}
}
