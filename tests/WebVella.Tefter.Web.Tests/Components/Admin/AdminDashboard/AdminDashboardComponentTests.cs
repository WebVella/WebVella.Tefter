namespace WebVella.Tefter.Web.Tests.Components;
public class AdminDashboardComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfAdminDashboard>();

		// Assert
		cut.Find(".tf-layout__body__main");

		Context.DisposeComponents();
	}
}
