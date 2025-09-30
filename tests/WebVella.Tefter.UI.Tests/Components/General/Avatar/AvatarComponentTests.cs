namespace WebVella.Tefter.UI.Tests.Components;
public class AvatarComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			var cssClass = "test-avatar";
			// Act
			var cut = Context.RenderComponent<TucAvatar>(parameters => parameters
			.Add(p => p.Class, cssClass)
			);

			// Assert
			cut.Find($".{cssClass}");

			Context.DisposeComponents();
		}
	}

}
