using System.Text.Json;

namespace WebVella.Tefter.Web.Tests.PageComponents;

public class SpaceViewPageComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfSpaceViewSpacePageAddon>(args => args
		.Add(x => x.Context, new TfSpacePageAddonContext()
		{
			ComponentOptionsJson = JsonSerializer.Serialize(new TfSpaceViewSpacePageAddonOptions())
		})
		);

		// Assert
		cut.Find(".fluent-messagebar-notification");

		Context.DisposeComponents();
	}

}
