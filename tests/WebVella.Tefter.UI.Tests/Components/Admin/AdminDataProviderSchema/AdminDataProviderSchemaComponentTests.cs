namespace WebVella.Tefter.UI.Tests.Components;
public class AdminDataProviderSchemaComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			// Act
			var cut = Context.RenderComponent<TfAdminDataProviderSchema>();

			// Assert
			cut.Find(".fluent-messagebar");

			Context.DisposeComponents();
		}
	}
}
