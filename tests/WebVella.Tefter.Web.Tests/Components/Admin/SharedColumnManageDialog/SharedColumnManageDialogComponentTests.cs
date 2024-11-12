namespace WebVella.Tefter.Web.Tests.Components;
public class SharedColumnManageDialogComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			var dialogProvider = Context.RenderComponent<FluentDialogProvider>();
			// Act
			 var dialogService = Context.Services.GetRequiredService<IDialogService>();
			var dialog = await dialogService.ShowDialogAsync<TfSharedColumnManageDialog>(new DialogParameters());
			// Assert
			dialogProvider.Find(".fluent-dialog-main");
			dialog.Dismiss(null);

			Context.DisposeComponents();
		}
	}
}
