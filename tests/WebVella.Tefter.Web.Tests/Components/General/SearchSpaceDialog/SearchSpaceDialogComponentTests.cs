namespace WebVella.Tefter.Web.Tests.Components;
public class SearchSpaceDialogComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			TfSpaceManagerMock.Setup(x => x.GetSpacesList()).Returns(Result.Ok(new List<TfSpace>()));
			TfSpaceManagerMock.Setup(x => x.GetAllSpaceViews()).Returns(Result.Ok(new List<TfSpaceView>()));

			// Act
			 var dialogService = Context.Services.GetRequiredService<IDialogService>();
			var dialog = await dialogService.ShowDialogAsync<TfSearchSpaceDialog>(new DialogParameters());
			// Assert
			dialogProvider.Find(".fluent-dialog-main");
			dialog.Dismiss(null);
		}
	}
}
