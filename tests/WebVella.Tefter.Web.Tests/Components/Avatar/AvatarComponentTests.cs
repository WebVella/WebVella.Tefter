using WebVella.Tefter.Web.Components;

namespace WebVella.Tefter.Web.Tests.Components.Avatar;


public class AvatarComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var cssClass = "test-avatar";
		// Act
		var cut = Context.RenderComponent<TfAvatar>(parameters => parameters
		.Add(p=> p.Class,cssClass)
		);

		// Assert
		cut.Find($".{cssClass}");
	}

}
