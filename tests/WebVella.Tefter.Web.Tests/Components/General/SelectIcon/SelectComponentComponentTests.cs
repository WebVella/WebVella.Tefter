namespace WebVella.Tefter.Web.Tests.Components;
public class SelectIconComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			// Act
			var cut = Context.RenderComponent<TfSelectIcon>(p => p
			.Add(x=> x.ValueChanged,(x) => { }));

			// Assert
			cut.Find(".tf-select-icon");

			Context.DisposeComponents();
		}
	}
}
