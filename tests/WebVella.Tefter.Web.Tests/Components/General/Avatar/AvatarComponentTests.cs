namespace WebVella.Tefter.Web.Tests.Components;

public class AvatarComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var cssClass = "test-avatar";
			// Act
			var cut = Context.RenderComponent<TfAvatar>(parameters => parameters
			.Add(p => p.Class, cssClass)
			);

			// Assert
			cut.Find($".{cssClass}");
		}
	}

}
