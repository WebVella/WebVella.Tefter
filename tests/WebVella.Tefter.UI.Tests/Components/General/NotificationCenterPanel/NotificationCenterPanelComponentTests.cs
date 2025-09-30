namespace WebVella.Tefter.UI.Tests.Components;
public class NotificationCenterPanelComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		var dialogProvider = Context.RenderComponent<FluentDialogProvider>();
		// Act
		var dialogService = Context.Services.GetRequiredService<IDialogService>();
		var dialog = await dialogService.ShowDialogAsync<TfNotificationCenterPanel>(new DialogParameters());
		//var cut = Context.RenderComponent<TfNotificationCenterPanel>();

		// Assert
		dialogProvider.Find(".notification-panel__body");
		dialog.Dismiss(null);

		Context.DisposeComponents();
	}
}
