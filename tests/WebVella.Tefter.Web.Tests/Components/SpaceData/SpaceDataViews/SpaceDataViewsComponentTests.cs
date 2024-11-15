namespace WebVella.Tefter.Web.Tests.Components;

public class SpaceDataViewsComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfSpaceDataViews>();

		// Assert
		cut.Find(".fluent-messagebar");

		Context.DisposeComponents();
	}

}
