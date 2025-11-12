namespace WebVella.Tefter.UI.Tests.Components;

public class EditorComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			// Act
			var cut = Context.Render<TucEditor>();

			// Assert
			cut.Find(".tf-editor");

			Context.Dispose();
		}
	}

}
