namespace WebVella.Tefter.UI.Tests.Components;
public class ProgressComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfProgress>(p => p
		.Add((x) => x.Visible, true)
		);

		// Assert
		cut.Find(".tf-progress");

		Context.DisposeComponents();
	}
}
