namespace WebVella.Tefter.Web.Tests.Components;
public class SearchSpaceDialogComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		var dialogProvider = Context.RenderComponent<FluentDialogProvider>();
		TfServiceMock.Setup(x => x.GetSpacesList()).Returns(new List<TfSpace>());
		TfServiceMock.Setup(x => x.GetAllSpaceViews()).Returns(new List<TfSpaceView>());

		// Act
		var dialogService = Context.Services.GetRequiredService<IDialogService>();
		var dialog = await dialogService.ShowDialogAsync<TfSearchSpaceDialog>(new DialogParameters());
		// Assert
		dialogProvider.Find(".fluent-dialog-main");
		dialog.Dismiss(null);

		Context.DisposeComponents();
	}
}
