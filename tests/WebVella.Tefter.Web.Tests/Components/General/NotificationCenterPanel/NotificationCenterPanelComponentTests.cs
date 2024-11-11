namespace WebVella.Tefter.Web.Tests.Components;
public class NotificationCenterPanelComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given

			// Act
			var cut = Context.RenderComponent<TfNotificationCenterPanel>();

			// Assert
			cut.Find(".tf-notification-center-panel");
		}
	}
}
