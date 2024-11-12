namespace WebVella.Tefter.Web.Tests.Components;
public class AvatarComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		var cssClass = "test-avatar";
		// Act
		var cut = Context.RenderComponent<TfAvatar>(parameters => parameters
		.Add(p => p.Class, cssClass)
		);

		// Assert
		cut.Find($".{cssClass}");

		Context.DisposeComponents();
	}

}
