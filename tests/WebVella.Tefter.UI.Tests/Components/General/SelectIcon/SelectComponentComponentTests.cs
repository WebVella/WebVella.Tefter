namespace WebVella.Tefter.UI.Tests.Components;
public class SelectIconComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfSelectIcon>(p => p
		.Add(x => x.ValueChanged, (x) => { }));

		// Assert
		cut.Find(".tf-select-icon");

		Context.DisposeComponents();
	}
}
