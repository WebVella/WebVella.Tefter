namespace WebVella.Tefter.Web.Tests.Components;
public class LocationComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			// Act
			var cut = Context.RenderComponent<TfLocation>();

			// Assert
			cut.Find(".location");

			Context.DisposeComponents();
		}
	}
}
