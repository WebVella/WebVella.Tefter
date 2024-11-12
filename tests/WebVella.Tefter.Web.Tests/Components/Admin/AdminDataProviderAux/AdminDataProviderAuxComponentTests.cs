namespace WebVella.Tefter.Web.Tests.Components;
public class AdminDataProviderAuxComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			// Act
			var cut = Context.RenderComponent<TfAdminDataProviderAux>();

			// Assert
			cut.Find(".fluent-messagebar");

			Context.DisposeComponents();
		}
	}
}
