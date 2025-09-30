using System.Data;
using System.Text.Json;
using WebVella.Tefter.UI.Layout;

namespace WebVella.Tefter.UI.Tests.PageComponents;

public class SpaceViewPageComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var authLayout = new TfAuthLayout();
		authLayout.NavigationState = new();
		var cut = Context.RenderComponent<TucSpaceViewSpacePageAddon>(args => args
		.Add(x => x.Context, new TfSpacePageAddonContext()
		{
			ComponentOptionsJson = JsonSerializer.Serialize(new TfSpaceViewSpacePageAddonOptions())
		})
		.Add(x=> x.TfAuthLayout, authLayout)
		);

		// Assert
		cut.Find(".fluent-messagebar");

		Context.DisposeComponents();
	}

}
