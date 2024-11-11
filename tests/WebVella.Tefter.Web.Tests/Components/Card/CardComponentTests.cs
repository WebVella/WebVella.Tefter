using WebVella.Tefter.Web.Components;

namespace WebVella.Tefter.Web.Tests.Components.Card;


public class CardComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var cssClass = "test-avatar";
		// Act
		var cut = Context.RenderComponent<TfCard>(parameters => parameters
		.Add(p=> p.Class,cssClass)
		);

		// Assert
		var mainWrapper = cut.Find($".tf-card");
		mainWrapper.HasAttribute("class");
		mainWrapper.ClassList.Contains(cssClass);
	}

}
