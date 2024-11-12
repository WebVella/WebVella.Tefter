namespace WebVella.Tefter.Web.Tests.Components;
public class ProgressComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			// Act
			var cut = Context.RenderComponent<TfProgress>(p => p
			.Add((x) => x.Visible, true)
			);

			// Assert
			cut.Find(".tf-progress");

			Context.DisposeComponents();
		}
	}
}
