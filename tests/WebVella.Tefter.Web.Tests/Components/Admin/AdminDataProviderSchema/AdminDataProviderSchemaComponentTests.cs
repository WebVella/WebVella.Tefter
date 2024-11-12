namespace WebVella.Tefter.Web.Tests.Components;
public class AdminDataProviderSchemaComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
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
