namespace WebVella.Tefter.Web.Tests.Components;
public class ValidationMessageComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var errors = new List<ValidationError>{ 
				new ValidationError("test","test")
			};
			// Act
			var cut = Context.RenderComponent<TfValidationMessage>(p => p
			.Add((x) => x.Errors, errors)
			.Add((x) => x.Field,"test")
			);

			// Assert
			cut.Find(".validation-message");
		}
	}
}
