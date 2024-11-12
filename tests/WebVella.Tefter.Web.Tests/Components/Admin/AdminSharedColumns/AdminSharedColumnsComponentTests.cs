namespace WebVella.Tefter.Web.Tests.Components;
public class AdminSharedColumnsComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfAdminSharedColumns>();

		// Assert
		cut.Find(".tf-admin-shared-columns-toolbar");

		Context.DisposeComponents();
	}
}
