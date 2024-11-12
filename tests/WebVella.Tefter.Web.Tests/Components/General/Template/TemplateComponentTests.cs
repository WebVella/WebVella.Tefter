namespace WebVella.Tefter.Web.Tests.Components;
public class TemplateComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			RenderFragment fragment = builder =>
						{
							builder.OpenElement(0, "header");
							builder.CloseElement();
						};
			// Act
			var cut = Context.RenderComponent<TfTemplate>(p => p
			.Add(x => x.ChildContent, fragment)
			);

			// Assert
			cut.Find("header");

			Context.DisposeComponents();
		}
	}

}
