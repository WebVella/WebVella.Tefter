namespace WebVella.Tefter.Web.Tests.Components;
public class CardComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var cssClass = "test-avatar";
			// Act
			var cut = Context.RenderComponent<TfCard>(parameters => parameters
			.Add(p => p.Class, cssClass)
			);

			// Assert
			var mainWrapper = cut.Find($".tf-card");
			mainWrapper.HasAttribute("class");
			mainWrapper.ClassList.Contains(cssClass);
		}
	}

}
