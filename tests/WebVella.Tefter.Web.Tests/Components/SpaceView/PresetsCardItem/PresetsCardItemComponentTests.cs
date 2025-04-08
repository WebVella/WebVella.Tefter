namespace WebVella.Tefter.Web.Tests.Components;

public class PresetsCardItemComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfQuickFiltersCardItem>(args => args
		.Add(x=> x.Item, new TucSpaceViewPreset()));

		// Assert
		cut.Find(".tf-grid-tr");

		Context.DisposeComponents();
	}

}
