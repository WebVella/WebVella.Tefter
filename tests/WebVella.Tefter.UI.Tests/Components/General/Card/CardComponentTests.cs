namespace WebVella.Tefter.UI.Tests.Components;
public class CardComponentTests : BaseTest
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
			var cut = Context.RenderComponent<TucCard>(parameters => parameters
			.Add(p => p.Class, cssClass)
			);

			// Assert
			var mainWrapper = cut.Find($".tf-card");
			mainWrapper.HasAttribute("class");
			mainWrapper.ClassList.Contains(cssClass);

			Context.DisposeComponents();
		}
	}

}
