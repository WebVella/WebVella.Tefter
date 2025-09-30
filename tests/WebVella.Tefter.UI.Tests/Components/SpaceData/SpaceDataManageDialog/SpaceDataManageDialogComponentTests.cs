namespace WebVella.Tefter.UI.Tests.Components;
public class SpaceDataManageDialogComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		var dialogProvider = Context.RenderComponent<FluentDialogProvider>();
		// Act
		var dialogService = Context.Services.GetRequiredService<IDialogService>();
		var dialog = await dialogService.ShowDialogAsync<TfSpaceDataManageDialog>(new TucSpaceData(),new DialogParameters());
		// Assert
		dialogProvider.Find(".fluent-dialog-main");
		dialog.Dismiss(null);

		Context.DisposeComponents();
	}
}
