namespace WebVella.Tefter.Web.Tests.Components;
public class SelectComponentComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given

			// Act
			var cut = Context.RenderComponent<TfSelectComponent<string>>();

			// Assert
			cut.Find(".tf-select-component");
		}
	}
}
