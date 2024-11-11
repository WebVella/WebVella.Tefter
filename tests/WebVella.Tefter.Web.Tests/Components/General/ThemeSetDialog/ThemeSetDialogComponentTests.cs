namespace WebVella.Tefter.Web.Tests.Components;
public class ThemeSetDialogComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given

			// Act
			 var dialogService = Context.Services.GetRequiredService<IDialogService>();
			var dialog = await dialogService.ShowDialogAsync<TfThemeSetDialog>(new DialogParameters());
			// Assert
			dialogProvider.Find(".fluent-dialog-main");
			dialog.Dismiss(null);
		}
	}
}
