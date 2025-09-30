namespace WebVella.Tefter.UI.Tests.Components;
public class AdminDataProviderJoinedDataComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			// Act
			var cut = Context.RenderComponent<TfAdminDataProviderJoinedData>();

			// Assert
			//When no data provider it shows nothings
			//cut.Find(".fluent-messagebar");

			Context.DisposeComponents();
		}
	}
}
