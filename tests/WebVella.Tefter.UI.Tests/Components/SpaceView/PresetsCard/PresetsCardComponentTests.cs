using WebVella.Tefter.UI.Components;

namespace WebVella.Tefter.UI.Tests.Components;

public class PresetsCardComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TucPresetFiltersCard>();

		// Assert
		cut.Find(".tf-card");

		Context.DisposeComponents();
	}

}
