namespace WebVella.Tefter.Web.Tests.Components;

using TfTemplate = WebVella.Tefter.Web.Components.TfTemplate;

public class TemplateComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		RenderFragment fragment = builder =>
					{
						builder.OpenElement(0, "header");
						builder.CloseElement();
					};
		// Act
		var cut = Context.RenderComponent<TfTemplateComponent>(p => p
		.Add(x => x.ChildContent, fragment)
		);

		// Assert
		cut.Find("header");

		Context.DisposeComponents();
	}

}
