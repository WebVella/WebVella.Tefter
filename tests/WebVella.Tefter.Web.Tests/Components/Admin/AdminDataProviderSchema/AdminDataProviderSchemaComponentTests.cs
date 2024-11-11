namespace WebVella.Tefter.Web.Tests.Components;
public class AdminDataProviderSchemaComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given

			// Act
			var cut = Context.RenderComponent<TfAdminDataProviderSchema>();

			// Assert
			cut.Find(".fluent-messagebar");
		}
	}
}
