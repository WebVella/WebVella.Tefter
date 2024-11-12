namespace WebVella.Tefter.Web.Tests.Components;
public class SelectColorComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			// Act
			var cut = Context.RenderComponent<TfSelectColor>(p => p
			.Add(x=> x.ValueChanged,(x) => { }));

			// Assert
			cut.Find(".tf-select-node");

			Context.DisposeComponents();
		}
	}
}
