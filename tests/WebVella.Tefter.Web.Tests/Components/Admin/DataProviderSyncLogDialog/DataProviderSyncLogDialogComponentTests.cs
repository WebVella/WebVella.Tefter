namespace WebVella.Tefter.Web.Tests.Components;
public class DataProviderSyncLogDialogComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		var dialogProvider = Context.RenderComponent<FluentDialogProvider>();
		// Act
		var dialogService = Context.Services.GetRequiredService<IDialogService>();
		var dialog = await dialogService.ShowDialogAsync<TfDataProviderSyncLogDialog>(new DialogParameters());
		// Assert
		dialogProvider.Find(".fluent-dialog-main");
		dialog.Dismiss(null);

		Context.DisposeComponents();
	}
}
