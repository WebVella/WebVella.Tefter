namespace WebVella.Tefter.Web.Tests.Components;
public class AdminDataProviderJoinedDataComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
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
