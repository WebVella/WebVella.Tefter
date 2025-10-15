using WebVella.Tefter.UI.Components;

namespace WebVella.Tefter.UI.Tests.Components;
public class PresetManageDialogComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		var dialogProvider = Context.RenderComponent<FluentDialogProvider>();
		// Act
		var dialogService = Context.Services.GetRequiredService<IDialogService>();
		var dialog = await dialogService.ShowDialogAsync<TucPresetFilterManageDialog>(new TfPresetFilterManagementContext(),new ());
		// Assert
		dialogProvider.Find(".fluent-dialog-main");
		dialog.Dismiss(null);

		Context.DisposeComponents();
	}
}
