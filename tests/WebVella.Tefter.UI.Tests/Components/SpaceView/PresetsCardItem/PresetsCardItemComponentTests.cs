using WebVella.Tefter.UI.Components;

namespace WebVella.Tefter.UI.Tests.Components;

public class PresetsCardItemComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TucPresetFiltersCardItem>(args => args
		.Add(x=> x.Item, new TfSpaceViewPreset()));

		// Assert
		cut.Find(".tf-grid-tr");

		Context.DisposeComponents();
	}

}
