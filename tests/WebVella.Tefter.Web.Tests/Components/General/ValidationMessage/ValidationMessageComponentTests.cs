using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.Web.Tests.Components;
public class ValidationMessageComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		var errors = new List<ValidationError>{
				new ValidationError("test","test")
			};
		// Act
		var cut = Context.RenderComponent<TfValidationMessage>(p => p
		.Add((x) => x.Errors, errors)
		.Add((x) => x.Field, "test")
		);

		// Assert
		cut.Find(".validation-message");

		Context.DisposeComponents();
	}
}
