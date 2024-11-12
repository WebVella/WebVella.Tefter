namespace WebVella.Tefter.Web.Tests.Components;
public class AdminFileRepositoryComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfAdminFileRepository>();

		// Assert
		cut.Find(".tf-admin-repository-toolbar");

		Context.DisposeComponents();
	}
}
