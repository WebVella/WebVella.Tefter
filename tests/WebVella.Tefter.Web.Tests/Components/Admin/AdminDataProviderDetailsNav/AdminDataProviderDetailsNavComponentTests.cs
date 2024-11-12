namespace WebVella.Tefter.Web.Tests.Components;
public class AdminDataProviderDetailsNavComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfAdminDataProviderDetailsNav>();

		// Assert
		cut.Find(".tf-tabnav");

		Context.DisposeComponents();
	}
}
