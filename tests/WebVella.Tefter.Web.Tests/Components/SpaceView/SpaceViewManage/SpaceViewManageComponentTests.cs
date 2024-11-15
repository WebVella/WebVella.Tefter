namespace WebVella.Tefter.Web.Tests.Components;

public class SpaceViewManageComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfSpaceViewManage>();

		// Assert
		cut.Find(".tf-layout__body__main");

		Context.DisposeComponents();
	}

}
