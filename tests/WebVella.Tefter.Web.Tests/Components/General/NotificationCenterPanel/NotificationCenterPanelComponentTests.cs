namespace WebVella.Tefter.Web.Tests.Components;
public class NotificationCenterPanelComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfNotificationCenterPanel>();

		// Assert
		cut.Find(".tf-notification-center-panel");

		Context.DisposeComponents();
	}
}
