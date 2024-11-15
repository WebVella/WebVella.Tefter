namespace WebVella.Tefter.Web.Tests.Components;

public class SpaceViewHeaderNavigationItemComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		var dict = new Dictionary<Guid, List<Guid>>();
		dict[Guid.Empty] = new();
		// Act
		var cut = Context.RenderComponent<TfSpaceViewHeaderNavigationItem>(args=>args
		.Add(x=> x.Item,new TucSpaceViewPreset())
		.Add(x=> x.SelectionDictionary,dict)
		);

		// Assert
		cut.Find("fluent-menu-item");

		Context.DisposeComponents();
	}

}
