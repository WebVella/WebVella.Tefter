namespace WebVella.Tefter.Web.Tests.Components;
public class AdminUserDetailsNavComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfAdminUserDetailsNav>();

		// Assert
		cut.Find(".tf-tabnav");

		Context.DisposeComponents();
	}
}
